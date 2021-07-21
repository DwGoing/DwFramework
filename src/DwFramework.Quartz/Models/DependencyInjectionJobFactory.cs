using System;
using Quartz;
using Quartz.Spi;

namespace DwFramework.Quartz
{
    public sealed class DependencyInjectionJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DependencyInjectionJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 返回IJob
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //Job类型
            Type jobType = bundle.JobDetail.JobType;
            //返回jobType对应类型的实例
            return _serviceProvider.GetService(jobType) as IJob;
        }

        /// <summary>
        /// 清理销毁IJob
        /// </summary>
        /// <param name="job"></param>
        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}