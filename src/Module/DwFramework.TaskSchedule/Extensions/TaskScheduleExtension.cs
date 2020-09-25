using System;

using DwFramework.Core;

namespace DwFramework.TaskSchedule
{
    public static class TaskScheduleExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterTaskScheduleService(this ServiceHost host) => host.RegisterType<TaskScheduleService>().SingleInstance();

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TaskScheduleService GetTaskScheduleService(this IServiceProvider provider) => provider.GetService<TaskScheduleService>();
    }
}
