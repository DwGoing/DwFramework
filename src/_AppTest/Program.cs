using System;
using System.Text;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.RPC;

using System.ServiceModel;
using Grpc.Net.Client;
using ProtoBuf;
using ProtoBuf.Grpc;
using ProtoBuf.Serializers;
using ProtoBuf.Grpc.Client;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Reflection;

namespace _AppTest
{
    [Service]
    public interface IA
    {
        [OperationContract]
        Response Do(Request request, CallContext context = default);
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

    [RPC(typeof(B))]
    public class A : IA
    {
        public A(B b)
        {
            Console.WriteLine(b.ID);
        }

        public Response Do(Request request, CallContext context)
        {
            return new Response() { Message = request.Message };
        }
    }

    public class B
    {
        public string ID = Guid.NewGuid().ToString();
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterRPCService(configPath: "RPC");
                host.RegisterType<B>().SingleInstance();
                host.OnInitializing += p =>
                {
                    var rpc = p.GetRPCService();
                };
                host.OnInitialized += p =>
                {
                    GrpcClientFactory.AllowUnencryptedHttp2 = true;
                    var channel = GrpcChannel.ForAddress("http://localhost:5021");
                    var client = channel.CreateGrpcService<IA>();
                    var res = client.Do(new Request() { Message = "Hello" });

                    channel = GrpcChannel.ForAddress("http://localhost:5021");
                    client = channel.CreateGrpcService<IA>();
                    res = client.Do(new Request() { Message = "Hello" });
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
