using System;
using System.Threading;

using DwFramework.Core;
using DwFramework.Web.Extensions;

namespace _Test.Web
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceHost host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                host.RegisterWebService();
                var provider = host.Build();
                provider.InitHttpServiceAsync<Startup>();
                provider.InitWebSocketServiceAsync(
                    onConnect: (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已连接");
                    },
                    onSend: (c, a) =>
                    {
                        Console.WriteLine($"向{c.ID}消息：{a.Message}");
                    },
                    onReceive: (c, a) =>
                    {
                        Console.WriteLine($"收到{c.ID}发来的消息：{a.Message}");
                    },
                    onClose: (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已断开");
                    });
                while (true) Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
