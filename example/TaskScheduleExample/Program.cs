using System;
using System.Threading.Tasks;
using Quartz;
using DwFramework.Core;
using DwFramework.TaskSchedule;

namespace TaskScheduleExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureTaskScheduleService();
            host.OnHostStarted += async p =>
            {
                var s = p.GetTaskScheduleService();
                await s.CreateSchedulerAsync("X");
                Console.WriteLine(await s.CreateJobAsync<Job>("X", "1/5 * * * * ?"));
            };
            await host.RunAsync();
        }
    }

    public class Job : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine(DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
