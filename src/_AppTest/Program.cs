using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.Socket;

using System.Text;
using System.Net;
using System.Net.Sockets;

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
                host.RegisterUdpService("Socket:Udp");
                host.OnInitializing += p =>
                {
                    var service = p.GetUdpService();
                    service.OnSend += a =>
                    {
                        Console.WriteLine($"发送消息:{Encoding.UTF8.GetString(a.Data)}");
                    };
                    service.OnReceive += a =>
                   {
                       var s = Encoding.UTF8.GetString(a.Data);
                       Console.WriteLine($"收{a.Remote}到消息：{s}");
                       _ = service.SendAsync(a.Remote, a.Data);
                   };
                    service.OnError += a =>
                    {
                        Console.WriteLine($"Erro:{a.Exception.Message}");
                    };
                };
                host.OnInitialized += p =>
                {
                    var c = new UdpClient(9999);
                    TaskManager.CreateTask(async () =>
                    {
                        var data = new byte[1024];
                        var a = await c.ReceiveAsync();
                        Console.WriteLine(Encoding.UTF8.GetString(a.Buffer));
                    });
                    var data = Encoding.UTF8.GetBytes("Hello World!");
                    c.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10200));
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