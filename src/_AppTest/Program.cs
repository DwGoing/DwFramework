using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.WebAPI;
using DwFramework.Rpc;
using DwFramework.Rpc.Cluster;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.RegisterWebAPIService<Startup>("webapi.json");
                host.AddJsonConfig("rpc.json");
                //host.RegisterRpcService("rpc.json");
                //host.RegisterClusterImpl();
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
