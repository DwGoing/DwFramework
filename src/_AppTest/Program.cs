using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.Socket;

using System.Net.Sockets;
using System.Net;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10100));
                //Console.WriteLine("1");
                //while (true)
                //{
                //    var str = Console.ReadLine();
                //    if (string.IsNullOrEmpty(str)) continue;
                //    if (str == "close")
                //    {
                //        client.Close();
                //        break;
                //    }
                //    client.Send(System.Text.Encoding.UTF8.GetBytes(str));
                //    Console.WriteLine("2");
                //}
                //Console.WriteLine("3");

                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterSocketService("Socket");
                host.OnInitializing += p =>
                {
                    var service = p.GetSocketService();
                    service.OnConnect += (c, a) =>
                    {
                        Console.WriteLine($"{c.ID}已连接");
                    };
                    service.OnSend += (c, a) =>
                    {
                        Console.WriteLine($"向{c.ID}消息：{System.Text.Encoding.UTF8.GetString(a.Data)}");
                    };
                    service.OnReceive += async (c, a) =>
                    {
                        var s = System.Text.Encoding.UTF8.GetString(a.Data);
                        Console.WriteLine($"收到{c.ID}发来的消息：{s}");
                        //if (!s.EndsWith("\r\n\r\n")) return;
                        //var data = new { A = "a", B = 123 }.ToJson();
                        //var msg = $"HTTP/1.1 200 OK\r\nContent-Type:application/json;charset=UTF-8\r\nContent-Length:{data.Length}\r\nConnection:close\r\n\r\n{data}";
                        //await service.SendAsync(c.ID, msg);
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
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}