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
                host.InitService(provider => provider.InitHttpServiceAsync<Startup>());
                //host.InitService(provider =>
                //{
                //    provider.InitWebSocketServiceAsync();
                //    var service = provider.GetWebService<WebSocketService>();
                //    service.OnConnect += (c, a) =>
                //    {
                //        Console.WriteLine($"{c.ID}已连接");
                //    };
                //    service.OnSend += (c, a) =>
                //    {
                //        Console.WriteLine($"向{c.ID}消息：{a.Message}");
                //    };
                //    service.OnReceive += (c, a) =>
                //    {
                //        Console.WriteLine($"收到{c.ID}发来的消息：{a.Message}");
                //    };
                //    service.OnClose += (c, a) =>
                //    {
                //        Console.WriteLine($"{c.ID}已断开");
                //    };
                //});
                //host.InitService(provider =>
                //{
                //    provider.InitSocketServiceAsync();
                //});
                //System.Threading.Tasks.Task.Run(() => { Thread.Sleep(10000); host.Stop(); });
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
