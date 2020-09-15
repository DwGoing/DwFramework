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

        /// <summary>
        /// 匹配DbType
        /// </summary>
        /// <param name="typeStr"></param>
        /// <returns></returns>
        public static DbType ParseDbType(this string typeStr)
        {
            foreach (var item in Enum.GetValues(typeof(DbType)))
            {
                if (string.Compare(item.ToString().ToLower(), typeStr.ToLower(), true) == 0)
                    return (DbType)item;
            }
            throw new Exception("无法找到匹配的DbType");
        }
    }
}
