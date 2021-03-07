using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;

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
                host.RegisterRPCService("RPC");
                host.RegisterClusterService("Test");
                host.OnInitializing += p =>
                {
                    var s = p.GetClusterService();
                    s.OnJoin += id => s.SyncData(DataType.Text, Encoding.UTF8.GetBytes($"Hello {id}"));
                    s.OnReceiveData += (id, type, data) => Console.WriteLine(Encoding.UTF8.GetString(data));
                    s.OnExit += id => Console.WriteLine($"{id} Exit");
                };
                host.OnInitialized += p =>
                {
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