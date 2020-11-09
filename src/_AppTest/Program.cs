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
using DwFramework.ORM;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterORMService("ORM.json");
                host.OnInitialized += p =>
                {
                    var service = p.GetORMService();
                    var conn = service.CreateConnection();
                    var res = conn.Queryable<dynamic>().AS("zsy").ToList();
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

    public enum A
    {
        X, Y, Z
    }
}
