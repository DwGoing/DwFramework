using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.WebSocket;

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
                host.OnInitializing += p =>
                {
                    var service = p.GetWebSocketService();
                    service.OnConnect += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已连接");
                    };
                    service.OnSend += (c, a) =>
                    {
                        Console.WriteLine($"向{c.ID}消息：{System.Text.Encoding.UTF8.GetString(a.Data)}");
                    };
                    service.OnReceive += (c, a) =>
                   {
                       var s = System.Text.Encoding.UTF8.GetString(a.Data);
                       Console.WriteLine($"收到{c.ID}发来的消息：{s}");
                       if (s == "c") _ = c.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.Empty);
                       //if (!s.EndsWith("\r\n\r\n")) return;
                       //var data = new { A = "a", B = 123 }.ToJson();
                       //var msg = $"HTTP/1.1 200 OK\r\nContent-Type:application/json;charset=UTF-8\r\nContent-Length:{data.Length}\r\nConnection:close\r\n\r\n{data}";
                       //await service.SendAsync(c.ID, msg);
                   };
                    service.OnClose += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已断开");
                    };
                    service.OnError += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID} {a.Exception.Message}");
                    };
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