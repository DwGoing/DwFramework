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
            var provider = host.Build();
            await provider.InitRpcServiceAsync();
            var rpc = provider.GetService<IRpcService, RpcService>();
            rpc.Service.Add<string>(T);

            var client = new Client("http://127.0.0.1:10010/");
            client.Invoke("T", new object[] { "helo" });

            Console.ReadLine();
        }

        public static void T(string str)
        {
            Console.WriteLine(str);
        }
    }
}
