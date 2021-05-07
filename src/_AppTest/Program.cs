using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
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
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("Config.json");
                host.RegisterLog();
                host.RegisterType<B>();
                host.RegisterRPCService("Rpc");
                host.OnInitializing += p =>
                {
                    var rpc = p.GetRPCService();
                    rpc.AddExternalService<B>();
                };
                host.OnInitialized += p =>
                {
                    HttpClient.DefaultProxy = new WebProxy();
                    GrpcClientFactory.AllowUnencryptedHttp2 = true;
                    var channel = GrpcChannel.ForAddress("http://localhost:10000");
                    var client = channel.CreateGrpcService<IA>();
                    var res = client.Do(new Request() { Message = "Hello" }).Result;
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

[RPC]
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

