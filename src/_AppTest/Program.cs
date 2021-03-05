using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.RPC;

using System.ServiceModel;
using Grpc.Net.Client;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf;
using ProtoBuf.Grpc.Client;

namespace _AppTest
{
    [Service]
    public interface IA
    {
        [OperationContract]
        ValueTask<Response> DoAsync(Request request, CallContext context = default);
    }

    [ProtoContract]
    public class Request
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }

    [ProtoContract]
    public class Response
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }

    public class A : IA
    {
        private B _b;

        public A(B b)
        {
            _b = b;
        }

        public async ValueTask<Response> DoAsync(Request request, CallContext context = default)
        {
            await Task.Delay(500);
            return new Response() { Message = $"{request.Message} {_b.Str}" };
        }
    }

    public class B
    {
        public string Str { get; set; }

        public B()
        {
            Str = Guid.NewGuid().ToString();
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
                //host.RegisterCluster(configPath: "Test");
                host.RegisterRPCService(configPath: "RPC");
                host.OnInitializing += p =>
                {
                    var s = p.GetRPCService();
                    s.AddDependentService<B>();
                    s.AddRpcImplement<A>();
                    //var s = p.GetCluster();
                    //s.OnJoin += id => Console.WriteLine(id);
                    //s.OnConnectBootPeerFailed += ex => Console.WriteLine(ex.Message);
                };
                host.OnInitialized += p =>
                {
                    static void X(string s)
                    {
                        GrpcClientFactory.AllowUnencryptedHttp2 = true;
                        using var channel = GrpcChannel.ForAddress("http://localhost:5021");
                        var client = channel.CreateGrpcService<IA>();
                        var res = client.DoAsync(new Request() { Message = s }).Result;
                        Console.WriteLine(res.Message);
                    }
                    X("Hello");
                    X("World");
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
