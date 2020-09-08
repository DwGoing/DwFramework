using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Rpc;
using DwFramework.Rpc.Plugins.Cluster;
using DwFramework.Rpc.Extensions;
using Grpc.Core;

namespace _Test.Rpc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(configFilePath: "Config.json");
                host.RegisterClusterImpl(args[0], bootPeer: args.Length > 1 ? args[1] : null);
                host.RegisterRpcService();
                host.OnInitializing += p =>
                {
                    var cluster = p.GetClusterImpl();
                    cluster.OnJoin += id => cluster.SyncData(Encoding.UTF8.GetBytes($"欢迎 {id} 加入集群"));
                    cluster.OnReceiveData += (id, data) => Console.WriteLine($"收到 {id} 消息:{Encoding.UTF8.GetString(data)}");
                };
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
