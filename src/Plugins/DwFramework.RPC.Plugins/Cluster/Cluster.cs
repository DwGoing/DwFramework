using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf.Grpc;

namespace DwFramework.RPC.Plugins
{
    public sealed class Cluster : ICluster
    {
        private readonly ClusterService _service;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        public Cluster(ClusterService service)
        {
            _service = service;
        }

        /// <summary>
        /// 加入集群
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<JoinResponse> JoinAsync(CallContext context = default) => _service.JoinAsync(context);

        /// <summary>
        /// 健康检查
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task HealthCheckAsync(CallContext context = default) => _service.HealthCheckAsync(context);

        /// <summary>
        /// 同步路由表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task SyncRouteTableAsync(Dictionary<string, string> request, CallContext context = default) => _service.SyncRouteTableAsync(request, context);

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task SyncDataAsync(SyncDataRequest request, CallContext context = default) => _service.SyncDataAsync(request, context);
    }
}
