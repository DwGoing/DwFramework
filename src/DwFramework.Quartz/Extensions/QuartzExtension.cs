using System;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.Quartz
{
    public static class QuartzExtension
    {
        /// <summary>
        /// 配置Quartz
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureQuartz(this ServiceHost host)
        {
            host.ConfigureContainer(builder =>
            {
                builder.RegisterType<QuartzService>().SingleInstance();
                builder.RegisterType<DependencyInjectionJobFactory>().SingleInstance();
            });
            return host;
        }

        /// <summary>
        /// 获取Quartz
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static QuartzService GetTaskSchedule(this IServiceProvider provider) => provider.GetService<QuartzService>();
    }
}
