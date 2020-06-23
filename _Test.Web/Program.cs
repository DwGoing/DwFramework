using System;
using System.Threading;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Web;
using DwFramework.Web.Extensions;
using DwFramework.Web.Plugins;

namespace _Test.Web
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceHost host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                //host.RegisterLog();
                host.RegisterWebService<HttpService>();
                //host.RegisterWebService<WebSocketService>();
                //host.RegisterWebService<SocketService>();
                var provider = host.Build();
                provider.InitHttpServiceAsync<Startup>();
                //provider.InitWebSocketServiceAsync();
                //var websocket = provider.GetWebService<WebSocketService>();
                //websocket.OnConnect += (c, a) =>
                //{
                //    Console.WriteLine($"{c.ID}已连接");
                //};
                //websocket.OnSend += (c, a) =>
                //{
                //    Console.WriteLine($"向{c.ID}消息：{a.Message}");
                //};
                //websocket.OnReceive += (c, a) =>
                //{
                //    Console.WriteLine($"收到{c.ID}发来的消息：{a.Message}");
                //};
                //websocket.OnClose += (c, a) =>
                //{
                //    Console.WriteLine($"{c.ID}已断开");
                //};
                //provider.InitSocketServiceAsync();
                while (true) Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
