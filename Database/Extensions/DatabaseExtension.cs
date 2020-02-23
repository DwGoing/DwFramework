using System;
using System.Threading.Tasks;

using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Database.Extensions
{
    public static class DatabaseExtension
    {
        /// <summary>
        /// 注册Database服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterDatabaseService(this ServiceHost host)
        {
            host.RegisterType<IDatabaseService, DatabaseService>().SingleInstance();
        }

        /// <summary>
        /// 初始化Database服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitDatabaseServiceAsync(this IServiceProvider provider)
        {
            return provider.GetService<IDatabaseService, DatabaseService>().OpenServiceAsync();
        }

        /// <summary>
        /// 匹配DbType
        /// </summary>
        /// <param name="typeStr"></param>
        /// <returns></returns>
        public static DbType ParseDbType(this string typeStr)
        {
            typeStr = typeStr.ToLower();
            foreach (var item in Enum.GetValues(typeof(DbType)))
            {
                if (item.ToString().ToLower() == typeStr)
                    return (DbType)item;
            }
            throw new Exception("无法找到匹配的DbType");
        }
    }
}
