using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.ORM
{
    public sealed class ORMService
    {
        private class Config
        {
            public class SlaveConnectionConfig
            {
                public string ConnectionString { get; set; }
                public int HitRate { get; set; }
            }

            public string ConnectionString { get; set; }
            public string DbType { get; set; }
            public SlaveConnectionConfig[] SlaveConnections { get; set; }
            public bool UseMemoryCache { get; set; } = false;

            public DbType ParseDbType()
            {
                if (string.IsNullOrEmpty(DbType)) throw new Exception("缺少DbType配置");
                foreach (var item in Enum.GetValues(typeof(DbType)))
                {
                    if (string.Compare(item.ToString().ToLower(), DbType.ToLower(), true) == 0)
                        return (DbType)item;
                }
                throw new Exception("无法找到匹配的DbType");
            }
        }

        private readonly Config _config;

        public SqlSugarClient DbConnection => CreateConnection();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="configKey"></param>
        public ORMService(Core.Environment environment, string configKey = null)
        {
            var configuration = environment.GetConfiguration(configKey ?? "Database");
            _config = configuration.GetConfig<Config>(configKey);
            if (_config == null) throw new Exception("未读取到Database配置");
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="initKeyType"></param>
        /// <returns></returns>
        public SqlSugarClient CreateConnection(InitKeyType initKeyType = InitKeyType.Attribute)
        {
            var config = new ConnectionConfig()
            {
                ConnectionString = _config.ConnectionString,//必填, 数据库连接字符串
                DbType = _config.ParseDbType(),         //必填, 数据库类型
                IsAutoCloseConnection = true,       //默认false, 自动关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = initKeyType,    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
                ConfigureExternalServices = new ConfigureExternalServices() // 配置扩展服务
            };
            if (_config.UseMemoryCache) config.ConfigureExternalServices.DataInfoCacheService = new DataMemoryCache(); // Memory缓存
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
        public Task<bool> CreateTableAsync(string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true) => TaskManager.CreateTask(() => DbConnection.DbMaintenance.CreateTable(tableName, columns, isCreatePrimaryKey));

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Task<bool> DropTableAsync(string tableName) => TaskManager.CreateTask(() => DbConnection.DbMaintenance.DropTable(tableName));

        /// <summary>
        /// 重置表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Task<bool> TruncateTableAsync(string tableName) => TaskManager.CreateTask(() => DbConnection.DbMaintenance.TruncateTable(tableName));

        /// <summary>
        /// 复制表结构
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public Task<bool> CopyTableStructAsync(string from, string to)
        {
            return TaskManager.CreateTask(() =>
            {
                var columns = DbConnection.DbMaintenance.GetColumnInfosByTableName(from);
                return DbConnection.DbMaintenance.CreateTable(to, columns);
            });
        }
    }
}