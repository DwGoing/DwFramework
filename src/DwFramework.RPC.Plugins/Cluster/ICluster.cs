using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc;

namespace DwFramework.RPC.Plugins
{
    [Service]
    public interface ICluster
    {
        [OperationContract]
        Task<JoinResponse> JoinAsync(CallContext context = default);
        [OperationContract]
        Task HealthCheckAsync(CallContext context = default);
        [OperationContract]
        Task SyncRouteTableAsync(Dictionary<string, string> request, CallContext context = default);
        [OperationContract]
        Task SyncDataAsync(SyncDataRequest request, CallContext context = default);
    }
}
