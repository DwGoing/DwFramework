using System;
using System.Text;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.Socket;
using DwFramework.WebAPI;
using DwFramework.WebSocket;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                //host.RegisterLog();
                host.RegisterWebSocketService("WebSocket.json");
                host.OnInitializing += provider =>
                {
                    var service = provider.GetWebSocketService();
                    service.OnConnect += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已连接");
                    };
                    service.OnSend += (c, a) =>
                    {
                        var msg = Encoding.UTF8.GetString(a.Data);
                        Console.WriteLine($"向{c.ID}发送消息：{msg}");
                    };
                    service.OnReceive += (c, a) =>
                    {
                        var msg = Encoding.UTF8.GetString(a.Data);
                        Console.WriteLine($"收到{c.ID}发来的消息：{msg}");
                        service.SendAsync(c.ID, a.Data);
                    };
                    service.OnClose += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已断开");
                    };
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
