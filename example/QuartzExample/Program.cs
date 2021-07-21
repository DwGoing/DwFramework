using System;
using System.Threading.Tasks;
using Quartz;
using Autofac;
using DwFramework.Core;
using DwFramework.Quartz;

namespace QuartzExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureContainer(b =>
            {
                b.RegisterType<A>().SingleInstance();
                b.RegisterType<Job>();
            });
            host.ConfigureQuartz();
            host.OnHostStarted += async p =>
            {
                var s = p.GetTaskSchedule();
                var ss = await s.CreateSchedulerAsync("X", true);
                await s.CreateJobAsync<Job>("X", "1/5 * * * * ?");
                await ss.Start();
            };
            await host.RunAsync();
        }
    }

    public class A
    {
        public readonly Guid Id = Guid.NewGuid();
    }

    public class Job : IJob
    {
        private A _a;

        public Job(A a)
        {
            _a = a;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine(_a.Id);
            return Task.CompletedTask;
        }
    }
}
