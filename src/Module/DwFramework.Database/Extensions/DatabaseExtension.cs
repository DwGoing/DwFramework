using System;
using SqlSugar;

using DwFramework.Core;

namespace DwFramework.Database
{
    public static class DatabaseExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterDatabaseService(this ServiceHost host) => host.RegisterType<DatabaseService>().SingleInstance();

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DatabaseService GetDatabaseService(this IServiceProvider provider) => provider.GetService<DatabaseService>();
    }
}
