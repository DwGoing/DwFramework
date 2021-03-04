using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.RPC;
using System.ServiceModel;
using System.Runtime.Serialization;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace _AppTest
{
    [ServiceContract]
    public interface IRpcInterface
    {
        ValueTask<Response> AuthLoginAsync(Request dto);
    }

    [DataContract]
    public class Request
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }

    [DataContract]
    public class Response
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }

    public class RpcInterface : IRpcInterface
    {
        public ValueTask<Response> AuthLoginAsync(Request dto)
        {
            return new ValueTask<Response>(new Response() { Message = dto.Message });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.AddJsonConfig("RPC.json");
                host.RegisterType<RpcInterface>();
                host.OnInitializing += p =>
                {
                    var s = p.GetRPCService();
                    s.AddService<RpcInterface, IRpcInterface>();
                };
                host.RegisterRPCService();
                host.OnInitialized += p =>
                {
                    GrpcClientFactory.AllowUnencryptedHttp2 = true;
                    using var http = GrpcChannel.ForAddress("http://localhost:5020");
                    var i = http.CreateGrpcService<IRpcInterface>();
                    var r = i.AuthLoginAsync(new Request() { Message = "Hello" }).Result;
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
