using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using SqlSugar;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.RPC;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using System.Threading.Tasks;
using Grpc.Core;

namespace _AppTest
{
    class Program
    {
        public class AService : A.ABase
        {
            public override Task<Response> Do(Request request, ServerCallContext context)
            {
                return Task.FromResult(new Response()
                {
                    Message = request.Message
                });
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterType<AService>();
                host.OnInitializing += p => p.GetRPCService().AddService(p.GetService<AService>());
                host.RegisterRPCService("RPC.json");
                host.OnInitialized += p =>
                {
                    var channel = new Channel("localhost:5020", ChannelCredentials.Insecure);
                    var client = new A.AClient(channel);
                    Console.WriteLine(client.Do(new Request() { Message = "123" }).Message);
                    channel.ShutdownAsync();
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
