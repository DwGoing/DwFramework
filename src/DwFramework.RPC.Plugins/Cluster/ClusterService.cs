using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

using DwFramework.Core;
using DwFramework.Core.Entities;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace DwFramework.RPC.Plugins
{
    public sealed class ClusterService : ConfigableServiceBase, ICluster
    {
        public sealed class Config
        {
            public string LinkUrl { get; init; }
            public int HealthCheckPerMs { get; init; } = 1000;
            public string BootPeer { get; init; }
        }

        private Config _config;
        private readonly ILogger<ClusterService> _logger;
        private Metadata _header;
        private Timer _healthCheckTimer;
        private readonly Dictionary<string, string> _peers = new();

        public readonly string ID;
        public event Action<Exception> OnError;
        public event Action<string> OnJoin;
        public event Action<string> OnExit;
        public event Action<string, DataType, byte[]> OnReceiveData;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public ClusterService(ILogger<ClusterService> logger = null)
        {
            _logger = logger;
            ID = RandomGenerater.RandomString(32);
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="config"></param>
        public void ReadConfig(Config config)
        {
            try
            {
                _config = config;
                if (_config == null) throw new Exception("未读取到Cluster配置");
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public void ReadConfig(string path = null, string key = null)
        {
            ReadConfig(ReadConfig<Config>(path, key));
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        public void Run()
        {
            _header = new Metadata
            {
                { "id", ID },
                { "linkurl", _config.LinkUrl }
            };
            _healthCheckTimer = new Timer(_config.HealthCheckPerMs);
            _healthCheckTimer.Elapsed += (_, args) => PeerHealthCheck();
            _healthCheckTimer.AutoReset = true;
            _healthCheckTimer.Start();
            _logger?.LogInformationAsync($"节点ID:{ID} ｜ 节点EndPoint:{_config.LinkUrl}");
            if (string.IsNullOrEmpty(_config.BootPeer)) return;
            UseRPC(_config.BootPeer, client =>
            {
                try
                {
                    var response = client.JoinAsync(new CallContext(new CallOptions(_header))).Result;
                    if (response == null || response.RemoteId == null) throw new Exception("无法获取节点信息");
                    _peers[response.RemoteId] = _config.BootPeer;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Cluster无法连接初始节点:{ex.Message}");
                }
            }, ex => OnError?.Invoke(ex));
        }

        /// <summary>
        /// 加入集群
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<JoinResponse> JoinAsync(CallContext context = default)
        {
            var id = context.RequestHeaders.Get("id");
            var url = context.RequestHeaders.Get("linkurl");
            if (string.IsNullOrEmpty(id?.Value) || url == null || string.IsNullOrEmpty(url?.Value)) throw new Exception($"无法获取节点信息");
            _peers[id.Value] = url.Value;
            SyncRouteTable();
            if (!PeerHealthCheck(id.Value)) throw new Exception("LinkUrl不可用");
            OnJoin?.Invoke(id.Value);
            return Task.FromResult(new JoinResponse() { RemoteId = ID });
        }

        /// <summary>
        /// 健康检查
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task HealthCheckAsync(CallContext context = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 同步路由表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task SyncRouteTableAsync(Dictionary<string, string> request, CallContext context = default)
        {
            request.ForEach(item =>
            {
                if (item.Key == ID) return;
                _peers[item.Key] = item.Value;
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task SyncDataAsync(SyncDataRequest request, CallContext context = default)
        {
            var id = context.RequestHeaders.Get("id");
            if (string.IsNullOrEmpty(id?.Value)) throw new Exception($"无法获取节点信息");
            OnReceiveData?.Invoke(id.Value, (DataType)request.Type, request.Hex.FromHex().Decompress(CompressType.LZ4).Result);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 调用RPC
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        private void UseRPC(string connectStr, Action<ICluster> action, Action<Exception> onException = null)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;
            var channel = GrpcChannel.ForAddress(connectStr);
            var cluster = channel.CreateGrpcService<ICluster>();
            try
            {
                action?.Invoke(cluster);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
            }
            finally
            {
                channel.ShutdownAsync();
            }
        }

        /// <summary>
        /// Peer健康检查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool PeerHealthCheck(string id)
        {
            var isOk = true;
            UseRPC(_peers[id], client =>
            {
                client.HealthCheckAsync().Wait();
            }, ex =>
            {
                _peers.Remove(id);
                isOk = false;
            });
            if (!isOk) OnExit?.Invoke(id);
            return isOk;
        }

        /// <summary>
        /// Peer健康检查
        /// </summary>
        private void PeerHealthCheck()
        {
            var ids = _peers.Keys.ToArray();
            foreach (var id in ids) PeerHealthCheck(id);
        }

        /// <summary>
        /// 同步路由表
        /// </summary>
        private void SyncRouteTable()
        {
            var request = new Dictionary<string, string>();
            _peers.ForEach(item => request.Add(item.Key, item.Value));
            _peers.ForEach(item => UseRPC(item.Value, client => client.SyncRouteTableAsync(request), _ => { }));
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void SyncData(DataType type, byte[] data)
        {
            var request = new SyncDataRequest()
            {
                Type = (int)type,
                Hex = data.Compress(CompressType.LZ4).Result.ToHex()
            };
            _peers.ForEach(item =>
                UseRPC(item.Value, client =>
                {
                    client.SyncDataAsync(request, new CallContext(new CallOptions(_header)));
                })
            );
        }
    }
}
