using System;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Grpc.Core;

using DwFramework.Core.Plugins;

namespace DwFramework.Rpc.Plugins.Cluster
{
    public class ClusterImpl : Cluster.ClusterBase
    {
        private readonly Metadata _header;
        private readonly Timer _healthCheckTimer;
        private readonly Dictionary<string, string> _peers = new Dictionary<string, string>();


        public readonly string ID;
        public readonly int HealthCheckPerMs;
        public event Action<string> OnJoin;
        public event Action<string> OnExit;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="linkUrl"></param>
        /// <param name="healthCheckPerMs"></param>
        /// <param name="bootPeer"></param>
        public ClusterImpl(string linkUrl, int healthCheckPerMs = 10000, string bootPeer = null)
        {
            ID = Generater.GenerateGUID().ToString();
            HealthCheckPerMs = healthCheckPerMs;
            _header = new Metadata
            {
                { "id", ID },
                { "linkurl", linkUrl }
            };
            _healthCheckTimer = new Timer(HealthCheckPerMs);
            _healthCheckTimer.Elapsed += (_, args) => PeerHealthCheck();
            _healthCheckTimer.AutoReset = true;
            _healthCheckTimer.Start();
            InitAsync(bootPeer);

            OnJoin += id => Console.WriteLine($"{id}加入集群");
            OnExit += id => Console.WriteLine($"{id}退出集群");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private Task InitAsync(string bootPeer)
        {
            return TaskManager.CreateTask(() =>
            {
                if (bootPeer == null) return;
                UseRPC(bootPeer, client =>
                {
                    client.Join(new Void(), _header);
                }, ex =>
                {
                    Console.WriteLine($"未加入集群");
                });
            });
        }

        /// <summary>
        /// 加入集群
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Void> Join(Void request, ServerCallContext context)
        {
            var id = context.RequestHeaders.Get("id");
            var url = context.RequestHeaders.Get("linkurl");
            if (id == null || string.IsNullOrEmpty(id.Value) || url == null || string.IsNullOrEmpty(url.Value)) throw new Exception($"无法获取节点信息");
            _peers[id.Value] = url.Value;
            OnJoin?.Invoke(ID);
            if (!PeerHealthCheck(id.Value)) throw new Exception("LinkUrl不可用");
            return Task.FromResult(new Void());
        }

        /// <summary>
        /// 健康检查
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Void> HealthCheck(Void request, ServerCallContext context)
        {
            return Task.FromResult(new Void());
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
                client.HealthCheck(new Void());
            }, ex =>
            {
                _peers.Remove(id);
                isOk = false;
                OnExit?.Invoke(ID);
            });
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
    }
}
