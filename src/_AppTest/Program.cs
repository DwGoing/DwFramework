using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using SqlSugar;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.WebAPI;
using DwFramework.WebSocket;
using DwFramework.Rpc;
using DwFramework.Rpc.Plugins;
using DwFramework.DataFlow;
using DwFramework.Database;
using Autofac;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int i = 0;
                var sf = new SnowflakeGenerater();
                while (i < 30)
                {
                    Console.WriteLine(sf.GenerateId());
                    i++;
                }


                //var host = new ServiceHost();
                //host.AddJsonConfig("Config.json");
                //host.Register<I>(c =>
                //{
                //    var a = c.Resolve<DwFramework.Core.Environment>();
                //    var type = a.GetConfiguration().GetConfig<int>("Type");
                //    return type switch
                //    {
                //        0 => new A(),
                //        1 => new B(),
                //        _ => throw new Exception()
                //    };
                //});
                //host.RegisterLog();
                #region WebAPI
                //host.RegisterWebAPIService<Startup>("WebAPI.json");
                #endregion
                #region WebSocket
                //host.RegisterWebSocketService("WebSocket.json");
                //host.OnInitialized += p =>
                //{
                //    var websocket = p.GetWebSocketService();
                //    websocket.OnReceive += (c, args) => Console.WriteLine(args.Message);
                //};
                #endregion
                #region Rpc
                //host.RegisterClusterImpl("Cluster.json");
                //host.RegisterRpcService("Rpc.json");
                //host.OnInitialized += p => p.GetClusterImpl().OnJoin += id => Console.WriteLine(id);
                #endregion
                //host.OnInitialized += p =>
                //{
                //    p.GetService<I>().Print();
                //};
                //host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }

    public interface I
    {
        public void Print();
    }

    public class A : I
    {
        public void Print()
        {
            Console.WriteLine("a");
        }
    }

    public class B : I
    {
        public void Print()
        {
            Console.WriteLine("b");
        }
    }
}
