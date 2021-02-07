using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.WebSocket;

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
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterWebSocketService("WebSocket");
                host.OnInitialized += p =>
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        p.StopWebSocketService();
                        Console.WriteLine(1);
                        await Task.Delay(5000);
                        _ = p.RunWebSocketServiceAsync("WebSocket1");
                        Console.WriteLine(2);
                    });
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}