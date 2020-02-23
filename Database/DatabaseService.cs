using System;
using System.Threading.Tasks;

using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Database
{
    public static class DatabaseServiceExtension
    {
        /// <summary>
        /// 注册RabbitMQ服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterDatabaseService(this ServiceHost host)
        {
            host.RegisterType<IDatabaseService, DatabaseService>().SingleInstance();
        }

        /// <summary>
        /// 初始化RabbitMQ服务
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

    public class DatabaseService : IDatabaseService
    {
        public class Config
        {
            public string ConnectionString { get; set; }
            public string DbType { get; set; }
        }

        private readonly IRunEnvironment _environment;
        private readonly Config _config;
        public SqlSugarClient Db { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="environment"></param>
        public DatabaseService(IRunEnvironment environment)
        {
            _environment = environment;
            _config = _environment.GetConfiguration().GetSection<Config>("Database");
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public Task OpenServiceAsync()
        {
            return Task.Run(() =>
            {
                Db = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = _config.ConnectionString,//必填, 数据库连接字符串
                    DbType = _config.DbType.ParseDbType(),         //必填, 数据库类型
                    IsAutoCloseConnection = true,       //默认false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                    InitKeyType = InitKeyType.SystemTable    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
                });
            });
        }
    }
}
