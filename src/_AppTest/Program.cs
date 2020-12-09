using System;
using System.Text.Json;
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
    class Program
    {
        [DisallowConcurrentExecution]
        class Job : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                var id = (int)context.MergedJobDataMap.Get("id");
                Console.WriteLine(id);
                return Task.CompletedTask;
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.RegisterTaskScheduleService();
                host.OnInitialized += p =>
                {
                    var s = p.GetLogger<Job>();
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
