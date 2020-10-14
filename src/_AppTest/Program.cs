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
        [Serializable]
        class A
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        static void Main(string[] args)
        {
            var a = new A().ToBytes();
            var b = a.ToObject<A>();

            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
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
                host.RegisterDataFlowService();
                host.OnInitialized += p =>
                {
                    var ser = p.GetDataFlowService();
                    var sum = 0;
                    var key = ser.CreateTaskQueue<int, int, int>(i => i, j => sum += j);
                    ser.GetTaskQueue(key);
                    for (var i = 0; i < 10000; i++) ser.AddInput(key, i);
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
