using System;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Grpc.Core;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.Rpc.Plugins
{
    public sealed class ClusterImpl : Cluster.ClusterBase
    {
        private class Config
        {
            public string LinkUrl { get; set; }
            public int HealthCheckPerMs { get; set; } = 10000;
            public string BootPeer { get; set; }
        }

        private readonly Config _config;
        private readonly Metadata _header;
        private readonly Timer _healthCheckTimer;
        private readonly Dictionary<string, string> _peers = new Dictionary<string, string>();


        public readonly string ID;
        public event Action<Exception> OnConnectBootPeerFailed;
        public event Action<string> OnJoin;
        public event Action<string> OnExit;
        public event Action<string, byte[]> OnReceiveData;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="configKey"></param>
        public ClusterImpl(Core.Environment environment, string configKey = null)
        {
            var configuration = environment.GetConfiguration(configKey ?? "Cluster");
            _config = configuration.GetConfig<Config>(configKey);
            if (_config == null) throw new Exception("未读取到Cluster配置");
            ID = Generater.GenerateRandomString(32);
            _header = new Metadata
            {
                { "id", ID },
                { "linkurl", _config.LinkUrl }
            };
            _healthCheckTimer = new Timer(_config.HealthCheckPerMs);
            _healthCheckTimer.Elapsed += (_, args) => PeerHealthCheck();
            _healthCheckTimer.AutoReset = true;
            _healthCheckTimer.Start();
            Console.WriteLine($"节点ID:{ID}");
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
            ID = Generater.GenerateRandomString(32);
            _header = new Metadata
            {
                { "id", ID },
                { "linkurl", _config.LinkUrl }
            };
            _healthCheckTimer = new Timer(_config.HealthCheckPerMs);
            _healthCheckTimer.Elapsed += (_, args) => PeerHealthCheck();
            _healthCheckTimer.AutoReset = true;
            _healthCheckTimer.Start();
            Console.WriteLine($"节点ID:{ID}");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (_config.BootPeer == null) return;
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
            if (id == null || string.IsNullOrEmpty(id.Value) || url == null || string.IsNullOrEmpty(url.Value)) throw new Exception($"无法获取节点信息");
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
                Console.WriteLine($"同步路由 {item.Key} {item.Value}");
                _peers[item.Key] = item.Value;
            });
            return Task.FromResult(new Empty());
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> SyncData(BytesValue request, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("id");
            if (id == null || string.IsNullOrEmpty(id.Value)) throw new Exception($"无法获取节点信息");
            OnReceiveData?.Invoke(id.Value, request.ToByteArray());
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
            Console.WriteLine($"{id} {(isOk ? "OK" : "BAD")}");
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
            var request = new RouteTable();
            _peers.ForEach(item => request.Value.Add(item.Key, item.Value));
            _peers.ForEach(item => UseRPC(item.Value, client => client.SyncRouteTableAsync(request)));
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="data"></param>
        public void SyncData(byte[] data)
        {
            var request = BytesValue.Parser.ParseFrom(data);
            _peers.ForEach(item => UseRPC(item.Value, client => client.SyncData(request, _header)));
        }
    }
}
