using System;
using System.Collections.Generic;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.WebAPI;
using DwFramework.WebSocket;
using DwFramework.Rpc;
using DwFramework.Rpc.Plugins;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var task = ParallelManager.Create(() => throw new Exception("1"), () => throw new Exception("2"));
                //task.Start(ex => Console.WriteLine(ex.Message));

                var host = new ServiceHost();
                host.RegisterLog();
                #region WebAPI
                //host.RegisterWebAPIService<Startup>("WebAPI.json");
                #endregion
                #region WebSocket
                host.RegisterWebSocketService("WebSocket.json");
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
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
