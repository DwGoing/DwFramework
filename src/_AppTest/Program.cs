using System;
using System.Text;
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
using Quartz;

namespace _AppTest
{
    class Program
    {
        class Job : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                Console.WriteLine(1);
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
                host.OnInitialized +=async p =>
                {
                    var service = p.GetTaskScheduleService();
                    await service.CreateScheduler("Test");
                    await service.CreateJobAsync<Job>("Test", "*/5 * * * * ?");
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
