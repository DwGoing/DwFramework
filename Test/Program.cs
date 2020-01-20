using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;
using AutoFac.Extras.NLog.DotNetCore;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Models;
using DwFramework.Http;
using DwFramework.WebSocket;
using DwFramework.Database;

namespace Test
{
    public interface ITestInterface
    {
        void TestMethod(string str);
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)] // 标记该类型实现的接口及实现类型
    public class TestClass1 : ITestInterface
    {
        private readonly ILogger _logger;

        public TestClass1(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("TestClass1已注入");
        }

        public void TestMethod(string str)
        {
            _logger.Debug($"TestClass1:{str}");
        }
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)] // 标记该类型实现的接口及实现类型
    public class TestClass2 : ITestInterface
    {
        private readonly ILogger _logger;

        public TestClass2(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("TestClass2已注入");
        }

        public void TestMethod(string str)
        {
            _logger.Debug($"TestClass2:{str}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost();
            host.RegisterConfiguration($"{Directory.GetCurrentDirectory()}", "Config.json");
            host.RegisterNLog();
            host.RegisterType<ITestInterface, TestClass2>();
            var provider = host.Build();
            var service = provider.GetService<ITestInterface>();
            service.TestMethod("helo");
            Console.ReadLine();

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
