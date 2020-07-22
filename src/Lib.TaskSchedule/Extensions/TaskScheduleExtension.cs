using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.TaskSchedule.Extensions
{
    public static class TaskScheduleExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterTaskScheduleService(this ServiceHost host)
        {
            host.RegisterType<TaskScheduleService>().SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TaskScheduleService GetTaskScheduleService(this IServiceProvider provider)
        {
            return provider.GetService<TaskScheduleService>();
        }
    }
}
