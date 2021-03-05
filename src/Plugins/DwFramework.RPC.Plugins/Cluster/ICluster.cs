using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace DwFramework.RPC.Plugins
{
    [Service]
    public interface ICluster
    {
        [OperationContract]
        Task<JoinResponse> JoinAsync(CallContext context = default);
        [OperationContract]
        void HealthCheck(CallContext context = default);
        [OperationContract]
        ValueTask SyncRouteTableAsync(Dictionary<string, string> request, CallContext context = default);
        [OperationContract]
        ValueTask SyncDataAsync(SyncDataRequest request, CallContext context = default);
    }
}
