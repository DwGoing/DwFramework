using System;
using System.IO;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Rpc;

using Hprose.RPC;

namespace _Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServiceHost host = new ServiceHost();
            host.RegisterConfiguration(Directory.GetCurrentDirectory(), "Config.json");
            host.RegisterRpcService();
            host.RegisterType<ITestInterface, TestClass1>();
            host.RegisterType<ITestInterface, TestClass2>();
            var provider = host.Build();
            await provider.InitRpcServiceAsync();
            var rpc = provider.GetService<IRpcService, RpcService>();
            rpc.RegisterFuncFromService<ITestInterface>();

            var client = new Client("http://127.0.0.1:10010/");
            client.Invoke("H", new object[] { "helo" });
            client.Invoke("X", new object[] { "helo" });

            Console.ReadLine();
        }
    }

    public interface ITestInterface
    {
        void TestMethod(string str);
    }

    public class TestClass1 : ITestInterface
    {
        public TestClass1()
        {
            Console.WriteLine("TestClass1已注入");
        }

        [Rpc("H")]
        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass1:{str}");
        }
    }

    public class TestClass2 : ITestInterface
    {
        public TestClass2()
        {
            Console.WriteLine("TestClass2已注入");
        }

        [Rpc("X")]
        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass2:{str}");
        }
    }
}