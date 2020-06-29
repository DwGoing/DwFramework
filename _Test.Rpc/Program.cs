using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Rpc;
using DwFramework.Rpc.Extensions;
using Hprose.RPC;

namespace _Test.Rpc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                host.RegisterInstance(new A());
                host.RegisterType<C>();
                host.RegisterRpcService();
                host.InitService(provider =>
                {
                    var rpc = provider.GetRpcService();
                    rpc.Service.AddMethod("AA", provider.GetService<A>());
                    rpc.Service.Add(B.BB);
                    rpc.RegisterFuncFromInstance(provider.GetService<C>());
                });
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    var client = new Client("http://127.0.0.1:10100");
                    client.Invoke("AA");
                    client.Invoke("BB");
                    client.Invoke("CC");
                });
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }

    public class A
    {
        [Rpc("AA")]
        public void AA()
        {
            Console.WriteLine("aa");
        }
    }

    public class B
    {
        public static void BB()
        {
            Console.WriteLine("bb");
        }
    }

    public class C
    {
        [Rpc("CC")]
        public void CC()
        {
            Console.WriteLine("cc");
        }
    }
}
