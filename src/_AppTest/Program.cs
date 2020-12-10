using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using DwFramework.RabbitMQ;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.Socket;
using DwFramework.TaskSchedule;
using DwFramework.WebAPI;
using DwFramework.WebSocket;
using Quartz;
using Microsoft.Extensions.Logging;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(configFilePath: "WebSocket.json");
                host.RegisterWebSocketService();
                host.OnInitialized += p =>
                {
                    var s = p.GetWebSocketService();
                    s.OnConnect += (a, c) => Console.WriteLine(a.ID);
                    s.OnReceive += async (a, c) =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(c.Data));
                        await a.CloseAsync();
                    };
                    s.OnClose += (a, c) => Console.WriteLine(a.ID);
                };
                host.OnInitialized += async p =>
                {
                    Thread.Sleep(2000);
                    var client = new WebSocketClient();
                    await client.ConnectAsync("ws://127.0.0.1:10090");
                    await client.SendAsync(Encoding.UTF8.GetBytes("XXX"));
                    await client.CloseAsync();
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
