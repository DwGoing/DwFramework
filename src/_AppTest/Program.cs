using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.RPC;

using System.ServiceModel;
using ProtoBuf;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Client;
using Grpc.Net.Client;

namespace _AppTest
{
    [Service]
    public interface IA
    {
        [OperationContract]
        Task<Response> Do(Request request, CallContext context = default);
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

        public Task<Response> Do(Request request, CallContext context = default)
        {
            return Task.FromResult(new Response() { Message = request.Message });
        }
    }

    public class B
    {
        public string ID = Guid.NewGuid().ToString();
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("Config.json");
                host.RegisterLog();
                host.RegisterType<B>().SingleInstance();
                host.RegisterRPCService("RPC");
                host.OnInitializing += p =>
                {
                    //var rpc = p.GetRPCService();
                    //rpc.AddInternalService(s => s.AddTransient<A>());
                    //rpc.AddRpcImplement<A>();
                };
                host.OnInitialized += p =>
                {
                    GrpcClientFactory.AllowUnencryptedHttp2 = true;
                    var channel = GrpcChannel.ForAddress("http://localhost:6002");
                    var client = channel.CreateGrpcService<IA>();
                    var res = client.Do(new Request() { Message = "Hello" }).Result;

                    channel = GrpcChannel.ForAddress("http://localhost:6002");
                    client = channel.CreateGrpcService<IA>();
                    res = client.Do(new Request() { Message = "Hello" }).Result;
                };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }
        }
    }
}