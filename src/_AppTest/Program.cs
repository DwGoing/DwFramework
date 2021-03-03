using System;
using Autofac;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.WebSocket;

namespace _AppTest
{
    public interface ITestInterface
    {
        void TestMethod(string str);
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass1 : ITestInterface
    {
        public TestClass1()
        {
            Console.WriteLine("TestClass1已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass1:{str}");
        }
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass2 : ITestInterface
    {
        public TestClass2()
        {
            Console.WriteLine("TestClass2已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass2:{str}");
        }
    }

    [Registerable(lifetime: Lifetime.Singleton, isAutoActivate: true)]
    public class X
    {
        public X(WebSocketService s)
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.AddJsonConfig("WebSocket.json");
                host.RegisterWebSocketService();
                // host.RegisterType<X>().SingleInstance().AutoActivate();
                host.RegisterFromAssemblies();
                host.OnInitialized += p =>
                {
                    var x =  p.GetService<X>();
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
