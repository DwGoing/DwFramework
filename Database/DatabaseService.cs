using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Database.Extensions;

namespace DwFramework.Database
{
    public class DatabaseService : BaseService
    {
        public class Config
        {
            public class SlaveConnectionConfig
            {
                public string ConnectionString { get; set; }
                public int HitRate { get; set; }
            }

            public string ConnectionString { get; set; }
            public string DbType { get; set; }
            public SlaveConnectionConfig[] SlaveConnections { get; set; }
        }

        private readonly Config _config;
        public SqlSugarClient DbConnection { get => CreateConnection(); }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="environment"></param>
        public DatabaseService(IServiceProvider provider, IEnvironment environment) : base(provider, environment)
        {
            _config = _environment.GetConfiguration().GetSection<Config>("Database");
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        public SqlSugarClient CreateConnection()
        {
            var config = new ConnectionConfig()
            {
                ConnectionString = _config.ConnectionString,//必填, 数据库连接字符串
                DbType = _config.DbType.ParseDbType(),         //必填, 数据库类型
                IsAutoCloseConnection = true,       //默认false, 时候知道关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = InitKeyType.SystemTable,    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
                ConfigureExternalServices = new ConfigureExternalServices() // 配置扩展服务
                {
                    DataInfoCacheService = new DataMemoryCache() // Memory缓存
                }
            };
            // 主从模式
            if (_config.SlaveConnections != null && _config.SlaveConnections.Length > 0)
            {
                config.SlaveConnectionConfigs = new List<SlaveConnectionConfig>();
                foreach (var item in _config.SlaveConnections)
                {
                    config.SlaveConnectionConfigs.Add(new SlaveConnectionConfig()
                    {
                        ConnectionString = item.ConnectionString,
                        HitRate = item.HitRate
                    });
                }
            }
            var db = new SqlSugarClient(config);
            if (db == null) throw new Exception("数据库连接创建异常");
            return db;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="isCreatePrimaryKey"></param>
        /// <returns></returns>
        public Task<bool> CreateTableAsync(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            return Task.Run(() =>
            {
                return DbConnection.DbMaintenance.CreateTable(tableName, columns, isCreatePrimaryKey);
            });
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Task<bool> DropTableAsync(string tableName)
        {
            return Task.Run(() =>
            {
                return DbConnection.DbMaintenance.DropTable(tableName);
            });
        }

        /// <summary>
        /// 复制表结构
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public Task<bool> CopyTableStructAsync(string from, string to)
        {
            return Task.Run(() =>
            {
                var columns = DbConnection.DbMaintenance.GetColumnInfosByTableName(from);
                return DbConnection.DbMaintenance.CreateTable(to, columns);
            });
        }
    }
}
