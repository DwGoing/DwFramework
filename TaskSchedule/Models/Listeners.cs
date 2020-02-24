using System.Threading;
using System.Threading.Tasks;

using Quartz;

namespace DwFramework.TaskSchedule
{
    public abstract class JobListener : IJobListener
    {
        public abstract string Name { get; }
        public abstract Task OnJobExecutionVetoed(IJobExecutionContext context);
        public abstract Task OnJobToBeExecuted(IJobExecutionContext context);
        public abstract Task OnJobWasExecuted(IJobExecutionContext context);

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return OnJobToBeExecuted(context);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            return OnJobWasExecuted(context);
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return OnJobExecutionVetoed(context);
        }
    }
}
