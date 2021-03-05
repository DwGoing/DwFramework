using System;
using System.Text;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterClusterService(configPath: "Test");
                host.RegisterRPCService(configPath: "RPC");
                host.OnInitializing += p =>
                {
                    var s = p.GetClusterService();
                    s.OnJoin += id => s.SyncData(DataType.Text, Encoding.UTF8.GetBytes("Hello"));
                    s.OnReceiveData += (id, type, bs) => Console.WriteLine(Encoding.UTF8.GetString(bs));
                };
                host.OnInitialized += p =>
                {

                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
