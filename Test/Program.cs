using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Http;
using DwFramework.WebSocket;
using DwFramework.Database;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost();
            host.RegisterConfiguration($"{Directory.GetCurrentDirectory()}/CoreTest", "Config.json");
            host.RegisterFromAssembly("Test"); // 从程序集注入
            var provider = host.Build();
            var service = provider.GetService<ITestInterface, TestClass1>();
            service.TestMethod("helo");

            //ServiceHost host = new ServiceHost();
            //host.RegisterConfiguration($"{Directory.GetCurrentDirectory()}/HttpTest", "Config.json");
            //host.RegisterHttpService();
            //var provider = host.Build();
            //provider.InitHttpService<HttpStartup>();

            //ServiceHost host = new ServiceHost();
            //host.RegisterConfiguration($"{Directory.GetCurrentDirectory()}/WebsocketTest", "Config.json");
            //host.RegisterWebSocketService();
            //var provider = host.Build();
            //provider.InitWebSocketService(async (msg, webSocket) =>
            //{
            //    // 对消息自定义处理
            //    Console.WriteLine(msg);
            //    byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            //    await webSocket.SendAsync(new ArraySegment<byte>(msgBytes), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);
            //    if (msg == "close")
            //        await webSocket.CloseOutputAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            //});

            //ServiceHost host = new ServiceHost();
            //host.RegisterConfiguration($"{Directory.GetCurrentDirectory()}/DatabaseTest", "Config.json");
            //host.RegisterDatabaseService();
            //var provider = host.Build();
            //provider.InitDatabaseService();
            //var service = provider.GetService<IDatabaseService, DatabaseService>();
            //var res = service.QueryWithCache<TestTable>(item => item.Id == 3);
            //Console.WriteLine(res.ToJson());
        }
    }
}
