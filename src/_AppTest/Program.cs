using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.WebSocket;

using System.Collections.Generic;
using System.Threading;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var host = new ServiceHost();
                //host.AddJsonConfig("WebSocket.json");
                //host.RegisterLog();
                //host.RegisterWebSocketService();
                //host.OnInitialized += p =>
                //{
                //    var w = p.GetWebSocketService();
                //    w.OnConnect += (c, a) => Console.WriteLine(c.ID);
                //    w.OnReceive += (c, a) =>
                //    {
                //        var d = System.Text.Encoding.UTF8.GetString(a.Data);
                //        Console.WriteLine(d);
                //        if (d == "exit") c.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.PolicyViolation).Wait();
                //    };

                //    var client = new WebSocketClient();
                //    client.OnClose += a =>
                //    {
                //        Console.WriteLine(a.CloseStatus);
                //    };
                //    client.ConnectAsync("ws://127.0.0.1:10090").Wait();
                //    client.SendAsync(System.Text.Encoding.UTF8.GetBytes("exit")).Wait();
                //};
                //host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
