using System;
using Autofac;

using DwFramework.Core;

namespace DwFramework.ORM
{
    public static class ORMExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterORMService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath, "Database");
                host.RegisterType<ORMService>().SingleInstance();
            }
            else host.Register(c => new ORMService(c.Resolve<Core.Environment>(), "Database")).SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ORMService GetDatabaseService(this IServiceProvider provider) => provider.GetService<ORMService>();
    }
}