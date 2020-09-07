using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Rpc;
using DwFramework.Rpc.Extensions;

namespace _Test.Rpc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                host.RegisterType<AService>();
                host.RegisterRpcService();
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
