using System;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.RPC.Plugins
{
    public sealed class ClusterService : ICluster
    {
        private sealed class Config
        {
            public string LinkUrl { get; set; }
            public int HealthCheckPerMs { get; set; } = 10000;
            public string BootPeer { get; set; }
        }

        private readonly Config _config;
        private readonly ILogger<ClusterService> _logger;
        private readonly Metadata _header;
        private readonly Timer _healthCheckTimer;
        private readonly Dictionary<string, string> _peers = new Dictionary<string, string>();

        public readonly string ID;
        public event Action<Exception> OnConnectBootPeerFailed;
        public event Action<string> OnJoin;
        public event Action<string> OnExit;
        public event Action<string, DataType, byte[]> OnReceiveData;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        /// <param name="logger"></param>
        public ClusterService(string configKey = null, string configPath = null, ILogger<ClusterService> logger = null)
        {
            _logger = logger;
            ID = RandomGenerater.RandomString(32);
            _config = ServiceHost.Environment.GetConfiguration<Config>(configKey, configPath);
            if (_config == null) throw new Exception("Cluster初始化异常 => 未读取到Cluster配置");
            _header = new Metadata
            {
                { "id", ID },
                { "linkurl", _config.LinkUrl }
            };
            _healthCheckTimer = new Timer(_config.HealthCheckPerMs);
            _healthCheckTimer.Elapsed += (_, args) => PeerHealthCheck();
            _healthCheckTimer.AutoReset = true;
            _healthCheckTimer.Start();
            _logger?.LogInformationAsync($"Cluster初始化 => 节点ID:{ID} ｜ 节点EndPoint:{_config.LinkUrl}");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
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
            }, ex => OnConnectBootPeerFailed?.Invoke(ex));
        }

        /// <summary>
        /// 加入集群
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<JoinResponse> JoinAsync(CallContext context = default)
        {
            try
            {
                var id = context.RequestHeaders.Get("id");
                var url = context.RequestHeaders.Get("linkurl");
                if (string.IsNullOrEmpty(id?.Value) || url == null || string.IsNullOrEmpty(url?.Value))
                    throw new Exception("无法获取节点信息");
                _peers[id.Value] = url.Value;
                SyncRouteTable();
                if (!PeerHealthCheck(id.Value)) throw new Exception("LinkUrl不可用");
                OnJoin?.Invoke(id.Value);
                return Task.FromResult(new JoinResponse() { RemoteId = ID });
            }
            catch (Exception ex)
            {
                throw new Exception($"Cluster加入集群失败:{ex.Message}");
            }
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
            if (id == null || string.IsNullOrEmpty(id.Value)) throw new Exception($"Cluster同步数据失败 => 无法获取节点信息");
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
                action(cluster);
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
            _peers.ForEach(item => UseRPC(item.Value, client => client.SyncRouteTableAsync(request)));
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
            _peers.ForEach(item => UseRPC(item.Value, client => client.SyncDataAsync(request, new CallContext(new CallOptions(_header)))));
        }
    }
}
