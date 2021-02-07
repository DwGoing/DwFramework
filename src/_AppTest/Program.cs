using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.RPC;

using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Autofac;

namespace _AppTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterRPCService("Rpc");
                host.OnInitialized += async p =>
                 {
                     await Task.Delay(5000);
                     await p.StopRpcServiceAsync();
                     Console.WriteLine(1);
                     await p.RunRPCServiceAsync("Rpc1");
                     Console.WriteLine(2);
                 };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}