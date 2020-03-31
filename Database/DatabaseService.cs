using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Database.Extensions;

namespace DwFramework.Database
{
    public class DatabaseService : ServiceApplication
    {
        public class Config
        {
            public string ConnectionString { get; set; }
            public string DbType { get; set; }
        }

        private readonly Config _config;
        public SqlSugarClient Db { get; private set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="environment"></param>
        public DatabaseService(IServiceProvider provider, IRunEnvironment environment) : base(provider, environment)
        {
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
                return Db.DbMaintenance.CreateTable(tableName, columns, isCreatePrimaryKey);
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
                return Db.DbMaintenance.DropTable(tableName);
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
                var columns = Db.DbMaintenance.GetColumnInfosByTableName(from);
                return Db.DbMaintenance.CreateTable(to, columns);
            });
        }
    }
}
