using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.WebSocket.Extensions;

namespace _Test.WebSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceHost host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                host.RegisterLog();
                host.RegisterWebSocketService();
                host.InitService(provider =>
                {
                    provider.InitWebSocketServiceAsync();
                    var service = provider.GetWebSocketService();
                    service.OnConnect += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已连接");
                    };
                    service.OnSend += (c, a) =>
                    {
                        Console.WriteLine($"向{c.ID}消息：{a.Message}");
                    };
                    service.OnReceive += (c, a) =>
                    {
                        Console.WriteLine($"收到{c.ID}发来的消息：{a.Message}");
                    };
                    service.OnClose += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已断开");
                    };
                });
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
