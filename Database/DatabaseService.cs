using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.Extensions.Configuration;
using Autofac.Extensions.DependencyInjection;
using CSRedis;
using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Database
{
    public static class DatabaseServiceExtension
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
        public static void InitDatabaseService(this AutofacServiceProvider provider)
        {
            provider.GetService<IDatabaseService, DatabaseService>();
        }
    }

    public class DatabaseService : IDatabaseService
    {
        public class Config
        {
            public RedisConfig Redis { get; set; }
            public MySqlConfig MySql { get; set; }
        }

        public class RedisConfig
        {
            public string ConnectString { get; set; }
        }

        public class MySqlConfig
        {
            public string MasterConnection { get; set; }
            public SlaveConnection[] SlaveConnections { get; set; }
        }

        public class SlaveConnection
        {
            public int HitRate { get; set; }
            public string ConnectString { get; set; }
        }

        public const string PREFIX = "DB_";

        private readonly IConfiguration _configuration;
        private readonly Config _config;

        public SqlSugarClient DbClient
        {
            get
            {
                List<SlaveConnectionConfig> slaveConnections = new List<SlaveConnectionConfig>();
                if (_config.MySql.SlaveConnections != null)
                    foreach (var item in _config.MySql.SlaveConnections)
                        slaveConnections.Add(new SlaveConnectionConfig { HitRate = item.HitRate, ConnectionString = item.ConnectString });
                return new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = _config.MySql.MasterConnection,
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true,
                    SlaveConnectionConfigs = slaveConnections.Count > 0 ? slaveConnections : null
                });
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public DatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
            _config = _configuration.GetSection("Database").Get<Config>();
            // 初始化Redis
            var csredis = new CSRedisClient(_config.Redis.ConnectString);
            RedisHelper.Initialization(csredis);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InsertWithCache<T>(T value) where T : class, new()
        {
            try
            {
                var cacheKey = $"{PREFIX}{typeof(T).Name}";
                DbClient.Ado.BeginTran();
                // 更新数据库
                if (DbClient.Insertable(value).ExecuteCommand() <= 0)
                    throw new Exception("无法插入到数据库");
                // 更新缓存
                RedisHelper.RPush(cacheKey, value);
                DbClient.Ado.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                DbClient.Ado.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InsertWithCache<T>(T[] value) where T : class, new()
        {
            try
            {
                var cacheKey = $"{PREFIX}{typeof(T).Name}";
                DbClient.Ado.BeginTran();
                // 更新数据库
                if (DbClient.Insertable(value).ExecuteCommand() <= 0)
                    throw new Exception("无法插入到数据库");
                // 更新缓存
                RedisHelper.RPush(cacheKey, value);
                DbClient.Ado.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                DbClient.Ado.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool DeleteWithCache<T>(T value) where T : class, new()
        {
            try
            {
                var cacheKey = $"{PREFIX}{typeof(T).Name}";
                DbClient.Ado.BeginTran();
                // 更新数据库
                if (DbClient.Deleteable(value).ExecuteCommand() <= 0)
                    throw new Exception("无法从数据库删除");
                // 更新缓存
                RedisHelper.LRem(cacheKey, 0, value);
                DbClient.Ado.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                DbClient.Ado.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool DeleteWithCache<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            try
            {
                var cacheKey = $"{PREFIX}{typeof(T).Name}";
                DbClient.Ado.BeginTran();
                T[] values = DbClient.Queryable<T>().Where(expression).ToArray();
                // 更新数据库
                if (DbClient.Deleteable(expression).ExecuteCommand() <= 0)
                    throw new Exception("无法从数据库删除");
                // 更新缓存
                foreach (var item in values)
                    RedisHelper.LRem(cacheKey, 0, item);
                DbClient.Ado.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                DbClient.Ado.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateWithCache<T>(T value) where T : class, new()
        {
            try
            {
                var cacheKey = $"{PREFIX}{typeof(T).Name}";
                DbClient.Ado.BeginTran();
                // 更新数据库
                if (DbClient.Updateable(value).ExecuteCommand() <= 0)
                    throw new Exception("无法从数据库删除");
                // 更新缓存
                RedisHelper.StartPipe()
                    .LRem(cacheKey, 0, value)
                    .LPush(cacheKey, value)
                    .EndPipe();
                DbClient.Ado.CommitTran();
                return true;
            }
            catch (Exception ex)
            {
                DbClient.Ado.RollbackTran();
                return false;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="forceFromDatabase"></param>
        /// <returns></returns>
        public T[] QueryWithCache<T>(bool forceFromDatabase = false)
        {
            try
            {
                var cacheKey = $"{PREFIX}{typeof(T).Name}";
                T[] values = null;
                if (!RedisHelper.Exists(cacheKey) || forceFromDatabase)
                {
                    values = DbClient.Queryable<T>().ToArray();
                    RedisHelper.StartPipe()
                        .Del(cacheKey)
                        .RPush(cacheKey, values)
                        .EndPipe();
                }
                else
                {
                    values = RedisHelper.LRange<T>(cacheKey, 0, -1);
                }
                return values;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="forceFromDatabase"></param>
        /// <returns></returns>
        public T[] QueryWithCache<T>(Expression<Func<T, bool>> expression, bool forceFromDatabase = false)
        {
            try
            {
                var cacheKey = $"{PREFIX}{typeof(T).Name}";
                T[] values = null;
                if (!RedisHelper.Exists(cacheKey) || forceFromDatabase)
                {
                    values = DbClient.Queryable<T>().Where(expression).ToArray();
                    RedisHelper.StartPipe()
                        .Del(cacheKey)
                        .RPush(cacheKey, values)
                        .EndPipe();
                }
                else
                {
                    values = RedisHelper.LRange<T>(cacheKey, 0, -1).Where(expression.Compile()).ToArray();
                }
                return values;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
