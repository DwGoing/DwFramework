using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf.Grpc;

namespace DwFramework.RPC.Plugins
{
    public sealed class Cluster : ICluster
    {
        private readonly ClusterService _service;

        public Cluster(ClusterService service)
        {
            _service = service;
        }

        public Task HealthCheckAsync(CallContext context = default) => _service.HealthCheckAsync(context);

        public Task<JoinResponse> JoinAsync(CallContext context = default) => _service.JoinAsync(context);

        public Task SyncDataAsync(SyncDataRequest request, CallContext context = default) => _service.SyncDataAsync(request, context);

        public Task SyncRouteTableAsync(Dictionary<string, string> request, CallContext context = default) => _service.SyncRouteTableAsync(request, context);
    }
}
