using System;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.TaskSchedule
{
    public static class TaskScheduleExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureTaskSchedule(this ServiceHost host)
        {
            host.ConfigureContainer(builder =>
            {
                builder.RegisterType<TaskScheduleService>().SingleInstance();
                builder.RegisterType<DependencyInjectionJobFactory>().SingleInstance();
            });
            return host;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TaskScheduleService GetTaskSchedule(this IServiceProvider provider) => provider.GetService<TaskScheduleService>();
    }
}
