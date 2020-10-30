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

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var s1 = "abcdefg";
                var s2 = "abxcdefg";
                Console.WriteLine(s1.ComputeSimilarity(s2));
                Console.ReadKey();

                var host = new ServiceHost();
                host.RegisterLog();
                host.RegisterDatabaseService("Database.json");
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
                host.OnInitialized += p =>
                {
                    var db = p.GetDatabaseService();
                    var connection = db.DbConnection;
                    connection.UseTran(() =>
                    {
                        var a = connection.Insertable(new T1()).ExecuteReturnEntity();
                        D(connection);
                    });
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
