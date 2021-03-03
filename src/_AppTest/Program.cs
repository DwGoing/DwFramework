using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.WebSocket;
using DwFramework.Socket;
using Autofac;

namespace _AppTest
{
    [Registerable(lifetime:Lifetime.Singleton,isAutoActivate:true)]
    public class A
    {
        public A(WebSocketService s)
        {

        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                // var c = new WebSocketClient();
                // await c.ConnectAsync("ws://localhost:6002");
                // await c.SendAsync(Encoding.UTF8.GetBytes("Hello"));
                var host = new ServiceHost();
                host.AddJsonConfig("Config.json");
                host.RegisterLog();
                host.RegisterTcpService("Tcp");
                host.RegisterWebSocketService("WebSocket");
                host.RegisterFromAssemblies();
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