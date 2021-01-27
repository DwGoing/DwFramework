using System;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.RPC.Plugins
{
    public sealed class Config
    {
        public string LinkUrl { get; set; }
        public int HealthCheckPerMs { get; set; } = 10000;
        public string BootPeer { get; set; }
    }

    public sealed class ClusterImpl : Cluster.ClusterBase
    {
        private readonly Config _config;
        private readonly ILogger<ClusterImpl> _logger;
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
        /// <param name="path"></param>
        /// <param name="key"></param>
        public ClusterImpl(string path = null, string key = null)
        {
            _config = ServiceHost.Environment.GetConfiguration<Config>(path, key);
            if (_config == null) throw new Exception("未读取到Cluster配置");
            _logger = ServiceHost.Provider.GetLogger<ClusterImpl>();
            ID = RandomGenerater.RandomString(32);
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
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="linkUrl"></param>
        /// <param name="healthCheckPerMs"></param>
        /// <param name="bootPeer"></param>
        public ClusterImpl(string linkUrl, int healthCheckPerMs = 10000, string bootPeer = null)
        {
            _config = new Config() { LinkUrl = linkUrl, HealthCheckPerMs = healthCheckPerMs, BootPeer = bootPeer };
            ID = RandomGenerater.RandomString(32);
            _header = new Metadata
            {
                { "id", ID },
                { "linkurl", _config.LinkUrl }
            };
            _healthCheckTimer = new Timer(_config.HealthCheckPerMs);
            _healthCheckTimer.Elapsed += (_, args) => PeerHealthCheck();
            _healthCheckTimer.AutoReset = true;
            _healthCheckTimer.Start();
            _logger?.LogInformationAsync($"节点ID:{ID} ｜ 节点EndPoint:{linkUrl}");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (string.IsNullOrEmpty(_config.BootPeer)) return;
            UseRPC(_config.BootPeer, client =>
            {
                var response = client.Join(new Empty(), _header);
                _peers[response.Value] = _config.BootPeer;
            }, ex => OnConnectBootPeerFailed?.Invoke(ex));
        }

        /// <summary>
        /// 加入集群
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<StringValue> Join(Empty request, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("id");
            var url = context.RequestHeaders.Get("linkurl");
            if (string.IsNullOrEmpty(id?.Value) || url == null || string.IsNullOrEmpty(url?.Value)) throw new Exception($"无法获取节点信息");
            _peers[id.Value] = url.Value;
            SyncRouteTable();
            if (!PeerHealthCheck(id.Value)) throw new Exception("LinkUrl不可用");
            OnJoin?.Invoke(id.Value);
            return Task.FromResult(new StringValue() { Value = ID });
        }

        /// <summary>
        /// 健康检查
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> HealthCheck(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 同步路由表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> SyncRouteTable(RouteTable request, ServerCallContext context)
        {
            request.Value.ForEach(item =>
            {
                if (item.Key == ID) return;
                _peers[item.Key] = item.Value;
            });
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> SendData(Data request, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("id");
            if (id == null || string.IsNullOrEmpty(id.Value)) throw new Exception($"无法获取节点信息");
            OnReceiveData?.Invoke(id.Value, (DataType)request.Type, request.Hex.FromHex().Decompress(CompressType.LZ4).Result);
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 调用RPC
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        private void UseRPC(string connectStr, Action<Cluster.ClusterClient> action, Action<Exception> onException = null)
        {
            var channel = new Channel(connectStr, ChannelCredentials.Insecure);
            try
            {
                var client = new Cluster.ClusterClient(channel);
                action(client);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
            }
            finally { channel.ShutdownAsync(); }
        }

        /// <summary>
        /// 调用RPC
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        private void UseRPC(string id, Action<Cluster.ClusterClient> action, Action<string, Exception> onException = null)
        {
            if (!_peers.ContainsKey(id)) return;
            UseRPC(_peers[id], action, ex =>
            {
                onException?.Invoke(id, ex);
                PeerHealthCheck(id);
            });
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
                client.HealthCheck(new Empty());
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
            ids.ForEach(item => PeerHealthCheck(item));
        }

        /// <summary>
        /// 同步路由表
        /// </summary>
        private void SyncRouteTable()
        {
            var request = new RouteTable();
            _peers.ForEach(item =>
            {
                request.Value.Add(item.Key, item.Value);
                UseRPC(item.Key, client => client.SyncRouteTableAsync(request), (id, ex) => { });
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void SendData(string id, DataType type, byte[] data)
        {
            if (!_peers.ContainsKey(id)) return;
            var request = new Data()
            {
                Type = (int)type,
                Hex = data.Compress(CompressType.LZ4).Result.ToHex()
            };
            UseRPC(id, client => client.SendDataAsync(request, _header), (id, ex) => { });
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void SyncData(DataType type, byte[] data)
        {
            _peers.ForEach(item => SendData(item.Key, type, data));
        }
    }
}
