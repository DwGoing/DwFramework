using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.Socket;

using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Autofac;

namespace _AppTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterTcpService("Tcp");
                host.RegisterUdpService("Udp");
                host.OnInitialized += p =>
                {
                    var s = p.GetUdpService();
                    s.OnReceive += a =>
                    {
                        var msg = Encoding.UTF8.GetString(a.Data);
                        Console.WriteLine(msg);
                        //var data = new { A = "a", B = 123 }.ToJson();
                        //var msg = $"HTTP/1.1 200 OK\r\nContent-Type:application/json;charset=UTF-8\r\nContent-Length:{data.Length}\r\nConnection:close\r\n\r\n{data}";
                        //_ = c.SendAsync(Encoding.UTF8.GetBytes(msg));
                    };
                };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}