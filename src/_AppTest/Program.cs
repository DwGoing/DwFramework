using System;
using System.Text;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using DwFramework.Socket;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterSocketService("Socket.json");
                host.OnInitializing += provider =>
                {
                    var service = provider.GetSocketService();
                    service.OnConnect += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已连接");
                    };
                    service.OnSend += (c, a) =>
                    {
                        Console.WriteLine($"向{c.ID}消息：{Encoding.UTF8.GetString(a.Data)}");
                    };
                    service.OnReceive += (c, a) =>
                    {
                        Console.WriteLine($"收到{c.ID}发来的消息：{Encoding.UTF8.GetString(a.Data)}");
                        var data = new { A = "a", B = 123 }.ToJson();
                        var msg = $"HTTP/1.1 200 OK\r\nContent-Type:application/json;charset=UTF-8\r\nContent-Length:{data.Length}\r\nConnection:close\r\n\r\n{data}";
                        service.SendAsync(c.ID, msg);
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
