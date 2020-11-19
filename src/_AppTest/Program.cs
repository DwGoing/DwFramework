using System;
using System.Collections.Generic;
using System.Threading;
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
        static object o = new object();

        static void Main(string[] args)
        {
            try
            {
                var hash = new HashSet<long>();
                var g1 = new SnowflakeGenerater(1);
                var g2 = new SnowflakeGenerater(2);
                var g3 = new SnowflakeGenerater(3);
                Task.Run(() =>
                {
                    while (true)
                    {
                        lock (o)
                        {
                            var id = g1.GenerateId();
                            if (hash.Contains(id)) Console.WriteLine($"{id} !!!");
                            else { hash.Add(id); Console.WriteLine($"{id}"); }
                        }
                        Thread.Sleep(1);
                    }
                });
                Task.Run(() =>
                {
                    while (true)
                    {
                        lock (o)
                        {
                            var id = g2.GenerateId();
                            if (hash.Contains(id)) Console.WriteLine($"{id} !!!");
                            else { hash.Add(id); Console.WriteLine($"{id}"); }
                        }
                        Thread.Sleep(1);
                    }
                });
                Task.Run(() =>
                {
                    while (true)
                    {
                        lock (o)
                        {
                            var id = g3.GenerateId();
                            if (hash.Contains(id)) Console.WriteLine($"{id} !!!");
                            else { hash.Add(id); Console.WriteLine($"{id}"); }
                        }
                        Thread.Sleep(1);
                    }
                });

                // var host = new ServiceHost();
                // host.RegisterLog();
                // host.AddJsonConfig("ORM.json");
                // host.RegisterType<ORMService>();
                // host.OnInitialized += p =>
                // {
                //     var s = p.GetORMService();
                //     var r= s.CreateConnection("DB2").Queryable<dynamic>().AS("zsy").ToDataTable();
                // };
                // host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
