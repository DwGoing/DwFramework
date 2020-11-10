using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using SqlSugar;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using System.Threading.Tasks;
using Grpc.Core;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterClusterImpl("Cluster.json");
                host.RegisterRPCService("RPC.json");
                host.OnInitializing += p =>
                {
                    var cluster = p.GetClusterImpl();
                    cluster.OnJoin += id => Console.WriteLine($"欢迎 {id} 加入集群");
                    cluster.OnExit += id => Console.WriteLine($"{id} 退出集群");
                    // SyncData() 在集群中同步数据
                    cluster.OnJoin += id => cluster.SyncData(DataType.Text, $"欢迎 {id} 加入集群".ToBytes(Encoding.UTF8));
                    cluster.OnReceiveData += (id, type, data) => Console.WriteLine($"收到 {id} 消息:{data.ToObject<string>(Encoding.UTF8)}");
                    cluster.OnConnectBootPeerFailed += ex => Console.WriteLine(ex);
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
