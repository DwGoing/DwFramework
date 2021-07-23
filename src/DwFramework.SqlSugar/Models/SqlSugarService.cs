using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using DwFramework.Core;

namespace DwFramework.SqlSugar
{
    public sealed class SqlSugarService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public SqlSugarService(IConfiguration configuration)
        {
            _configuration = configuration;
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
            var config = _configuration.Get<Config>();
            if (config == null) throw new NotFoundException("缺少SqlSugar配置");
            if (!config.ConnectionConfigs.ContainsKey(connName)) throw new Exception("找不到该连接的配置");
            var connConfig = config.ConnectionConfigs[connName];
            var connectionConfig = new ConnectionConfig()
            {
                ConnectionString = connConfig.ConnectionString,//必填, 数据库连接字符串
                DbType = connConfig.DbType,         //必填, 数据库类型
                IsAutoCloseConnection = true,       //默认false, 自动关闭数据库连接, 设置为true无需使用using或者Close操作
                InitKeyType = initKeyType,    //默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
                ConfigureExternalServices = new ConfigureExternalServices() // 配置扩展服务
            };
            if (connConfig.UseMemoryCache) connectionConfig.ConfigureExternalServices.DataInfoCacheService = new DataMemoryCache(); // Memory缓存
                                                                                                                                    // 主从模式
            if (connConfig.SlaveConnections != null && connConfig.SlaveConnections.Length > 0)
            {
                connectionConfig.SlaveConnectionConfigs = new List<SlaveConnectionConfig>();
                foreach (var item in connConfig.SlaveConnections)
                {
                    connectionConfig.SlaveConnectionConfigs.Add(new SlaveConnectionConfig()
                    {
                        ConnectionString = item.ConnectionString,
                        HitRate = item.HitRate
                    });
                }
            }
            if (entityNameService != null) connectionConfig.ConfigureExternalServices.EntityNameService = entityNameService;
            if (entityService != null) connectionConfig.ConfigureExternalServices.EntityService = entityService;
            var db = new SqlSugarClient(connectionConfig);
            if (db == null) throw new Exception("无法创建连接");
            return db;
        }
    }
}