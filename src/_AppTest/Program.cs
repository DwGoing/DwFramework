using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.Socket;

namespace _AppTest
{
    public class A
    {
        public int a { get; set; }
        public string b { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("Config.json");
                host.RegisterLog();
                host.RegisterTcpService("Tcp");
                host.OnInitialized += p =>
                {
                    var s = p.GetTcpService();
                    s.OnConnect += (c, e) => Console.WriteLine(1);
                    s.OnReceive += (c, e) =>
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(e.Data));
                        var data = new { A = "a", B = 123 }.ToJson();
                        var msg = $"HTTP/1.1 200 OK\r\nContent-Type:application/json;charset=UTF-8\r\nContent-Length:{data.Length}\r\nConnection:close\r\n\r\n{data}";
                        _ = c.SendAsync(Encoding.UTF8.GetBytes(msg));
                    };
                    s.OnClose += (c, e) => Console.WriteLine($"{c.ID}已断开");
                };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }
        }
    }
}