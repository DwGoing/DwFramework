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
        class A
        {
            public long ID { get; set; }
            public DateTime Date { get; set; }
            public Sex Sex { get; set; }
        }

        enum Sex
        {
            Unknow,
            男,
            女
        }

        static Dictionary<Type, Func<object, object>> ConvertFunc = new Dictionary<Type, Func<object, object>>()
        {
            {typeof(MySql.Data.Types.MySqlDateTime),src=>DateTime.Parse(src.ToString()) }
        };

        static Dictionary<string, Func<object, object>> PropertyFunc = new Dictionary<string, Func<object, object>>()
        {
            {"Date",src=>DateTime.Parse(src.ToString()) }
        };

        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.RegisterORMService("ORM.json");
                host.OnInitialized += p =>
                {
                    var s = p.GetORMService();
                    var db = s.CreateConnection();
                    var dt = db.Ado.GetDataTable($"SELECT id as ID,date as Date,sex as Sex FROM _test");
                    var res = dt.ToArray<A>(propertyFunc: PropertyFunc);
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
