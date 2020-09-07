using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Rpc;
using DwFramework.Rpc.Extensions;
using Grpc.Core;

namespace _Test.Rpc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(configFilePath: "Config.json");
                host.RegisterType<AService>();
                host.RegisterRpcService();
                host.OnInitialized += p =>
                {
                    Thread.Sleep(3000);
                    Channel channel = null;
                    try
                    {
                        channel = new Channel("127.0.0.1:5000", ChannelCredentials.Insecure);
                        var client = new A.AClient(channel);
                        var response = client.Do(new Request() { Message = "123" });
                        Console.WriteLine(response.Message);
                    }
                    finally
                    {
                        channel?.ShutdownAsync();
                    }
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
