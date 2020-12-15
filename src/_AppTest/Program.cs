using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using DwFramework.RabbitMQ;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.Socket;
using DwFramework.TaskSchedule;
using DwFramework.WebAPI;
using DwFramework.WebSocket;
using Quartz;
using Microsoft.Extensions.Logging;

namespace _AppTest
{
    class AA
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string IsOk { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var host = new ServiceHost(configFilePath: "WebAPI.json");
                //host.RegisterLog();
                //host.RegisterWebAPIService<Startup>();
                //host.Run();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
