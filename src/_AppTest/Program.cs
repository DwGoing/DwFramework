using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.Socket;
using DwFramework.TaskSchedule;
using DwFramework.WebAPI;
using DwFramework.WebSocket;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.AddJsonConfig("ORM.json");
                host.RegisterType<ORMService>();
                host.OnInitialized += p =>
                {
                    var s = p.GetORMService();
                    var r= s.CreateConnection("DB2").Queryable<dynamic>().AS("zsy").ToDataTable();
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
