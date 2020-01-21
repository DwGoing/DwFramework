using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Net.WebSockets;

using Microsoft.Extensions.DependencyInjection;
using AutoFac.Extras.NLog.DotNetCore;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Models;
using DwFramework.Http;
using DwFramework.WebSocket;
using DwFramework.Database;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost();
            host.RegisterConfiguration($"{Directory.GetCurrentDirectory()}", "Config.json");
            host.RegisterWebSocketService();
            var provider = host.Build();
            provider.InitWebSocketServiceAsync(
                onConnect: (c, a) =>
                {
                    Console.WriteLine($"{c.ID}已连接");
                },
                onSend: (c, a) =>
               {
                   var msg = a.Message;
                   Console.WriteLine($"向{c.ID}消息：{msg}");
               },
                onReceive: async (c, a) =>
                {
                    var msg = a.Message;
                    Console.WriteLine($"收到{c.ID}发来的消息：{msg}");
                    if (msg == "close")
                    {
                        await c.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    }
                },
                onClose: (c, a) =>
                {
                    Console.WriteLine($"{c.ID}已断开");
                });
            Console.ReadLine();
        }
    }
}
