using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace DwFramework.TaskSchedule
{
    public class ScheduleListener : IJobListener
    {
        public string Name => throw new NotImplementedException();

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
