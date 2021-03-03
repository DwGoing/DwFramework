using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.WebSocket;
using DwFramework.Socket;
using Autofac;

namespace _AppTest
{
    public sealed class WebSocketMessage
    {
        public string Method { get; set; }
        public object[] Params { get; set; }
    }

    public sealed class LoginInfo
    {
        public string Type { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Passowrd { get; set; }
        public string Uid { get; set; }
        public string OriginCode { get; set; }
        public string AccountType { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var c = new WebSocketClient();
                c.OnReceive += args =>
                {
                    var res = args.Data.FromJsonBytes<ResultInfo>();
                };
                await c.ConnectAsync("ws://localhost:6002");
                await c.SendAsync(new WebSocketMessage()
                {
                    Method = "Auth.Login",
                    Params = new object[] { new LoginInfo(){
                        Type="t",
                        Username = "commkit",
                        Passowrd = "123456",
                        Uid="330624197207315918",
                        OriginCode="3306240011",
                        AccountType="2"
                    }}
                }.ToJsonBytes());
                Console.Read();
                // var host = new ServiceHost();
                // host.AddJsonConfig("Config.json");
                // host.RegisterLog();
                // host.RegisterTcpService("Tcp");
                // host.RegisterWebSocketService("WebSocket");
                // host.RegisterFromAssemblies();
                // host.OnInitialized += p =>
                // {

                // };
                // await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }
        }
    }
}