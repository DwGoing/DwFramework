using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace DwFramework.TaskSchedule
{
    public abstract class ScheduleListener : ISchedulerListener
    {
        public abstract Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default);
        public abstract Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default);
        public abstract Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default);
        public abstract Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default);
        public abstract Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default);
        public abstract Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default);
        public abstract Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default);
        public abstract Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default);
        public abstract Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default);
        public abstract Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default);
        public abstract Task SchedulerInStandbyMode(CancellationToken cancellationToken = default);
        public abstract Task SchedulerShutdown(CancellationToken cancellationToken = default);
        public abstract Task SchedulerShuttingdown(CancellationToken cancellationToken = default);
        public abstract Task SchedulerStarted(CancellationToken cancellationToken = default);
        public abstract Task SchedulerStarting(CancellationToken cancellationToken = default);
        public abstract Task SchedulingDataCleared(CancellationToken cancellationToken = default);
        public abstract Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default);
        public abstract Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default);
        public abstract Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default);
        public abstract Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default);
        public abstract Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default);
    }

    public abstract class JobListener : IJobListener
    {
        public abstract string Name { get; }

        public abstract Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default);
        public abstract Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default);
        public abstract Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default);
    }

    public abstract class TriggerListener : ITriggerListener
    {
        public abstract string Name { get; }

        public abstract Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default);
        public abstract Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default);
        public abstract Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default);
        public abstract Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default);
    }
}
