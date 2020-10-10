using System;
using Autofac;

using DwFramework.Core;

namespace DwFramework.Database
{
    public static class DatabaseExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterDatabaseService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath, "Database");
                host.RegisterType<DatabaseService>().SingleInstance();
            }
            else host.Register(c => new DatabaseService(c.Resolve<Core.Environment>(), "Database")).SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DatabaseService GetDatabaseService(this IServiceProvider provider) => provider.GetService<DatabaseService>();
    }
}
