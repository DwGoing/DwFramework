using System;
using System.Collections.Generic;
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
                host.RegisterWebAPIService<Startup>("WebAPI.json");
                //host.RegisterClusterImpl("Cluster.json");
                //host.RegisterRpcService("Rpc.json");
                //host.OnInitialized += p => p.GetClusterImpl().OnJoin += id => Console.WriteLine(id);
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
