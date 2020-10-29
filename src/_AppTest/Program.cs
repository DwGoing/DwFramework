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
    [Serializable]
    class A
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var a = new A() { id = 1, name = "dwgoing" };
                var b = a.ToBytes();
                var c = b.Compress(CompressType.LZ4).Result;
                var d = c.Decompress(CompressType.LZ4).Result;

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

        static void D(SqlSugarClient connection)
        {
            throw new Exception();
            connection.Insertable(new T2() { }).ExecuteCommand();
        }
    }

    [SugarTable("test1")]
    public class T1
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
        public int ID { get; set; }
        [SugarColumn(ColumnName = "val")]
        public string Value { get; set; }
    }

    [SugarTable("test2")]
    public class T2
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
        public int ID { get; set; }
        [SugarColumn(ColumnName = "val")]
        public string Value { get; set; }
    }
}
