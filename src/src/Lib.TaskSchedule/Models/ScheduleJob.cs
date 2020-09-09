using System.Threading.Tasks;

using Quartz;

namespace DwFramework.TaskSchedule
{
    public abstract class ScheduleJob : IJob
    {
        public abstract Task Execute(IJobExecutionContext context);
    }
}
