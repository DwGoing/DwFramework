using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.WebAPI;
using DwFramework.WebSocket;
using DwFramework.Rpc;
using DwFramework.Rpc.Plugins;
using DwFramework.DataFlow;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var filter = new BloomFilter(100000);
                for (var i = 1; i < 5000; i++)
                {
                    filter.Add(IdentificationGenerater.RandomString(6));
                }
                filter.Add("dwgoing");
                Console.WriteLine(filter.IsExist("dwgoing1"));
                Console.ReadKey();

                //var host = new ServiceHost();
                //host.RegisterLog();
                //#region WebAPI
                ////host.RegisterWebAPIService<Startup>("WebAPI.json");
                //#endregion
                //#region WebSocket
                ////host.RegisterWebSocketService("WebSocket.json");
                ////host.OnInitialized += p =>
                ////{
                ////    var websocket = p.GetWebSocketService();
                ////    websocket.OnReceive += (c, args) => Console.WriteLine(args.Message);
                ////};
                //#endregion
                //#region Rpc
                ////host.RegisterClusterImpl("Cluster.json");
                ////host.RegisterRpcService("Rpc.json");
                ////host.OnInitialized += p => p.GetClusterImpl().OnJoin += id => Console.WriteLine(id);
                //#endregion
                //host.RegisterDataFlowService();
                //host.OnInitialized += p =>
                //{
                //    var val = 1;
                //    switch (val)
                //    {
                //        case 1:
                //            goto A;
                //    }
                //A:
                //    Console.WriteLine("a");
                //};
                //host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
