using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqlSugar;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.ORM
{
    public sealed class ORMService : ConfigableService
    {
        public sealed class Config
        {
            public Dictionary<string, DbConnectionConfig> ConnectionConfigs { get; init; }
        }

        public sealed class DbConnectionConfig
        {
            public string ConnectionString { get; init; }
            public string DbType { get; init; }
            public SlaveDbConnectionConfig[] SlaveConnections { get; init; }
            public bool UseMemoryCache { get; init; } = false;

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

        public sealed class SlaveDbConnectionConfig
        {
            public string ConnectionString { get; init; }
            public int HitRate { get; init; }
        }

        private readonly ILogger<ORMService> _logger;
        private Config _config;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public ORMService(ILogger<ORMService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public void ReadConfig(string path = null, string key = null)
        {
            try
            {
                _config = ReadConfig<Config>(path, key);
                if (_config == null) throw new Exception("未读取到ORM配置");
            }
            catch (Exception ex)
            {
                _ = _logger.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="connName"></param>
        /// <returns></returns>
        public DbType GetDbType(string connName)
        {
            if (!_config.ConnectionConfigs.ContainsKey(connName)) throw new Exception("未知数据库");
            return _config.ConnectionConfigs[connName].ParseDbType();
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="initKeyType"></param>
        /// <param name="entityNameService"></param>
        /// <param name="entityService"></param>
        /// <returns></returns>
        public SqlSugarClient CreateConnection(string connName, InitKeyType initKeyType = InitKeyType.Attribute, Action<Type, EntityInfo> entityNameService = null, Action<PropertyInfo, EntityColumnInfo> entityService = null)
        {
            try
            {
                if (!_config.ConnectionConfigs.ContainsKey(connName)) return null;
                var connConfig = _config.ConnectionConfigs[connName];
                var config = new ConnectionConfig()
                {
                    ConnectionString = connConfig.ConnectionString,//必填, 数据库连接字符串
                    DbType = connConfig.ParseDbType(),         //必填, 数据库类型
                    IsAutoCloseConnection = true,       //默认false, 自动关闭数据库连接, 设置为true无需使用using或者Close操作
                    InitKeyType = initKeyType,    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
                    ConfigureExternalServices = new ConfigureExternalServices() // 配置扩展服务
                };
                if (connConfig.UseMemoryCache) config.ConfigureExternalServices.DataInfoCacheService = new DataMemoryCache(); // Memory缓存
                                                                                                                              // 主从模式
                if (connConfig.SlaveConnections != null && connConfig.SlaveConnections.Length > 0)
                {
                    config.SlaveConnectionConfigs = new List<SlaveConnectionConfig>();
                    foreach (var item in connConfig.SlaveConnections)
                    {
                        config.SlaveConnectionConfigs.Add(new SlaveConnectionConfig()
                        {
                            ConnectionString = item.ConnectionString,
                            HitRate = item.HitRate
                        });
                    }
                }
                if (entityNameService != null) config.ConfigureExternalServices.EntityNameService = entityNameService;
                if (entityService != null) config.ConfigureExternalServices.EntityService = entityService;
                var db = new SqlSugarClient(config);
                if (db == null) throw new Exception("数据库连接创建异常");
                return db;
            }
            catch (Exception ex)
            {
                _ = _logger.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <param name="isCreatePrimaryKey"></param>
        /// <returns></returns>
        public Task<bool> CreateTableAsync(string connName, string tableName, List<DbColumnInfo> columns, bool isCreatePrimaryKey = true)
        {
            return TaskManager.CreateTask(() => CreateConnection(connName).DbMaintenance.CreateTable(tableName, columns, isCreatePrimaryKey));
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Task<bool> DropTableAsync(string connName, string tableName)
        {
            return TaskManager.CreateTask(() => CreateConnection(connName).DbMaintenance.DropTable(tableName));
        }

        /// <summary>
        /// 重置表
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Task<bool> TruncateTableAsync(string connName, string tableName)
        {
            return TaskManager.CreateTask(() => CreateConnection(connName).DbMaintenance.TruncateTable(tableName));
        }

        /// <summary>
        /// 复制表结构
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public Task<bool> CopyTableStructAsync(string connName, string from, string to)
        {
            return TaskManager.CreateTask(() =>
            {
                var connection = CreateConnection(connName);
                var columns = connection.DbMaintenance.GetColumnInfosByTableName(from);
                return connection.DbMaintenance.CreateTable(to, columns);
            });
        }
    }
}