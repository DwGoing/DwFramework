using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using DwFramework.Core;

namespace DwFramework.Quartz
{
    public sealed class QuartzService
    {
        private readonly DependencyInjectionJobFactory _dependencyInjectionJobFactory;

        /// <summary>
        /// 构造函数
        /// </summary>
        public QuartzService(DependencyInjectionJobFactory dependencyInjectionJobFactory)
        {
            _dependencyInjectionJobFactory = dependencyInjectionJobFactory;
        }

        /// <summary>
        /// 获取调度器
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task<IScheduler> GetSchedulerAsync(string schedulerName)
        {
            var scheduler = await DirectSchedulerFactory.Instance.GetScheduler(schedulerName);
            if (scheduler == null) throw new Exception("未知调度器");
            return scheduler;
        }

        /// <summary>
        /// 创建调度器
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task<IScheduler> CreateSchedulerAsync(string schedulerName, bool dependencyInjectionJob = false)
        {
            DirectSchedulerFactory.Instance.CreateScheduler(
                schedulerName,
                Guid.NewGuid().ToString(),
                new DefaultThreadPool(),
                new RAMJobStore()
            );
            var scheduler = await GetSchedulerAsync(schedulerName);
            if (dependencyInjectionJob) scheduler.JobFactory = _dependencyInjectionJobFactory;
            return scheduler;
        }

        /// <summary>
        /// 添加ScheduleListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task AddScheduleListenerAsync<T>(string schedulerName) where T : ISchedulerListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddSchedulerListener(new T());
        }

        /// <summary>
        /// 添加JobListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task AddJobListenerAsync<T>(string schedulerName) where T : IJobListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddJobListener(new T(), GroupMatcher<JobKey>.AnyGroup());
        }

        /// <summary>
        /// 添加JobListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task AddJobListenerAsync<T>(string schedulerName, string name, string group) where T : IJobListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddJobListener(new T(), KeyMatcher<JobKey>.KeyEquals(new JobKey(name, group)));
        }

        /// <summary>
        /// 添加JobListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task AddJobListenerForGroupAsync<T>(string schedulerName, string group) where T : IJobListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddJobListener(new T(), GroupMatcher<JobKey>.GroupContains(group));
        }

        /// <summary>
        /// 添加JobListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task AddJobListenerForNameAsync<T>(string schedulerName, string name) where T : IJobListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddJobListener(new T(), NameMatcher<JobKey>.NameContains(name));
        }

        /// <summary>
        /// 添加TriggerListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task AddTriggerListenerAsync<T>(string schedulerName) where T : ITriggerListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddTriggerListener(new T(), GroupMatcher<TriggerKey>.AnyGroup());
        }

        /// <summary>
        /// 添加TriggerListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task AddTriggerListenerAsync<T>(string schedulerName, string name, string group) where T : ITriggerListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddTriggerListener(new T(), KeyMatcher<JobKey>.KeyEquals(new TriggerKey(name, group)));
        }

        /// <summary>
        /// 添加TriggerListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task AddTriggerListenerForGroupAsync<T>(string schedulerName, string group) where T : ITriggerListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddTriggerListener(new T(), GroupMatcher<TriggerKey>.GroupContains(group));
        }

        /// <summary>
        /// 添加TriggerListener
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task AddTriggerListenerForNameAsync<T>(string schedulerName, string name) where T : ITriggerListener, new()
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            scheduler.ListenerManager.AddTriggerListener(new T(), NameMatcher<TriggerKey>.NameContains(name));
        }

        /// <summary>
        /// 释放调度器
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        public async Task DisposeSchedulerAsync(string schedulerName)
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            await scheduler.Shutdown();
        }

        /// <summary>
        /// 每日排除
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="calName"></param>
        /// <param name="startHour"></param>
        /// <param name="startMinute"></param>
        /// <param name="startSecond"></param>
        /// <param name="endHour"></param>
        /// <param name="endMinute"></param>
        /// <param name="endSecond"></param>
        /// <returns></returns>
        public async Task ExcludeInDayAsync(string schedulerName, string calName, int startHour = 0, int startMinute = 0, int startSecond = 0, int endHour = 23, int endMinute = 59, int endSecond = 59)
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            var calender = new DailyCalendar(DateBuilder.DateOf(startHour, startMinute, startSecond).DateTime, DateBuilder.DateOf(endHour, endMinute, endSecond).DateTime);
            await scheduler.AddCalendar(calName, calender, true, true);
        }

        /// <summary>
        /// 每周排除
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="calName"></param>
        /// <param name="excludeDays"></param>
        /// <returns></returns>
        public async Task ExcludeInWeekAsync(string schedulerName, string calName, DayOfWeek[] excludeDays)
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            var calender = new WeeklyCalendar();
            excludeDays.ForEach(item => calender.SetDayExcluded(item, true));
            await scheduler.AddCalendar(calName, calender, true, true);
        }

        /// <summary>
        /// 每月排除
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="calName"></param>
        /// <param name="excludeDays"></param>
        /// <returns></returns>
        public async Task ExcludeInMonthAsync(string schedulerName, string calName, int[] excludeDays)
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            var calender = new MonthlyCalendar();
            excludeDays.ForEach(item => calender.SetDayExcluded(item, true));
            await scheduler.AddCalendar(calName, calender, true, true);
        }

        /// <summary>
        /// 每年排除
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="calName"></param>
        /// <param name="excludeMonthDays"></param>
        /// <returns></returns>
        public async Task ExcludeInYearAsync(string schedulerName, string calName, (int, int)[] excludeMonthDays)
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            var calender = new AnnualCalendar();
            excludeMonthDays.ForEach(item => calender.SetDayExcluded(DateBuilder.DateOf(0, 0, 0, item.Item2, item.Item1).DateTime, true));
            await scheduler.AddCalendar(calName, calender, true, true);
        }

        /// <summary>
        /// 根据Cron表达式排除
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="calName"></param>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public async Task ExcludeByCronAsync(string schedulerName, string calName, string cronExpression)
        {
            var scheduler = await GetSchedulerAsync(schedulerName);
            var calender = new CronCalendar(cronExpression);
            await scheduler.AddCalendar(calName, calender, true, true);
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="jobBuild"></param>
        /// <param name="triggerBuild"></param>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        private async Task<DateTimeOffset> CreateJobAsync<T>(string schedulerName, Func<JobBuilder, JobBuilder> jobBuild, Func<TriggerBuilder, TriggerBuilder> triggerBuild, string jobName = null, string jobGroup = null, string triggerName = null, string triggerGroup = null) where T : IJob
        {

            var scheduler = await GetSchedulerAsync(schedulerName);
            var jobBuilder = jobBuild(JobBuilder.Create<T>());
            if (jobName != null)
            {
                JobKey key = new JobKey(jobName);
                if (jobGroup != null) key.Group = jobGroup;
                jobBuilder.WithIdentity(key);
            }
            var jobDetails = jobBuilder.Build();
            var triggerBuilder = triggerBuild(TriggerBuilder.Create());
            if (triggerName != null)
            {
                TriggerKey key = new TriggerKey(triggerName);
                if (triggerGroup != null) key.Group = triggerGroup;
                triggerBuilder.WithIdentity(key);
            }
            var trigger = triggerBuilder.Build();
            return await scheduler.ScheduleJob(jobDetails, trigger);
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="repeat"></param>
        /// <param name="intervalMilliseconds"></param>
        /// <param name="startAt"></param>
        /// <param name="calName"></param>
        /// <param name="properties"></param>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> CreateJobAsync<T>(string schedulerName, int repeat, long intervalMilliseconds, DateTimeOffset? startAt = null, string calName = null, IDictionary<string, object> properties = null, string jobName = null, string jobGroup = null, string triggerName = null, string triggerGroup = null) where T : IJob
        {
            if (repeat < 0 || intervalMilliseconds <= 0) throw new Exception("参数错误");
            return CreateJobAsync<T>(schedulerName, jobBuilder =>
             {
                 if (properties != null) jobBuilder.SetJobData(new JobDataMap(properties));
                 return jobBuilder;
             }, triggerBuilder =>
             {
                 triggerBuilder.WithSimpleSchedule(builder =>
                 {
                     if (repeat == 0) builder.RepeatForever();
                     else builder.WithRepeatCount(repeat);
                     builder.WithInterval(TimeSpan.FromMilliseconds(intervalMilliseconds));
                 });
                 triggerBuilder.StartAt(startAt == null || startAt.Value.Subtract(DateTimeOffset.Now).Ticks < 0 ? DateTimeOffset.Now : startAt.Value);
                 if (calName != null)
                     triggerBuilder.ModifiedByCalendar(calName);
                 return triggerBuilder;
             }, jobName, jobGroup, triggerName, triggerGroup);
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedulerName"></param>
        /// <param name="cronExpression"></param>
        /// <param name="startAt"></param>
        /// <param name="calName"></param>
        /// <param name="properties"></param>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        public Task<DateTimeOffset> CreateJobAsync<T>(string schedulerName, string cronExpression, DateTimeOffset? startAt = null, string calName = null, IDictionary<string, object> properties = null, string jobName = null, string jobGroup = null, string triggerName = null, string triggerGroup = null) where T : IJob
        {
            return CreateJobAsync<T>(schedulerName, jobBuilder =>
             {
                 if (properties != null) jobBuilder.SetJobData(new JobDataMap(properties));
                 return jobBuilder;
             }, triggerBuilder =>
             {
                 triggerBuilder.WithCronSchedule(cronExpression);
                 triggerBuilder.StartAt(startAt == null || startAt.Value.Subtract(DateTimeOffset.Now).Ticks < 0 ? DateTimeOffset.Now : startAt.Value);
                 if (calName != null) triggerBuilder.ModifiedByCalendar(calName);
                 return triggerBuilder;
             }, jobName, jobGroup, triggerName, triggerGroup);
        }
    }
}
