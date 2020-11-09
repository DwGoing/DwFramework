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
using DwFramework.ORM.Plugins;

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
                host.RegisterRepositories();
                host.OnInitialized += p =>
                {
                    var service = p.GetService<Rep>(); ;
                    var res = service.FindAllAsync().Result;
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

    [Repository]
    public class Rep : BaseRepository<A>
    {

    }

    [SugarTable("zsy")]
    public class A
    {
        [SugarColumn(ColumnName = "xm")]
        public string Name { get; set; }
    }
}
