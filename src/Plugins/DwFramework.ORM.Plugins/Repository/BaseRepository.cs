using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using SqlSugar;

namespace DwFramework.ORM.Plugins
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        private readonly ORMService _ormService;
        private readonly string _connName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ormService"></param>
        /// <param name="connName"></param>
        public BaseRepository(ORMService ormService, string connName)
        {
            _ormService = ormService;
            _connName = connName;
            if (_ormService == null) throw new Exception("未找到ORM服务");
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        protected SqlSugarClient CreateConnection()
        {
            return _ormService.CreateConnection(_connName);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Select(Expression<Func<T, bool>> expression = null, SqlSugarClient conn = null)
        {
            conn ??= CreateConnection();
            var queryable = conn.Queryable<T>();
            if (expression != null) queryable = queryable.Where(expression);
            return queryable;
        }

        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="identity"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public async Task<List<T>> InsertOrUpdateAsync(List<T> objs, Expression<Func<T, object>> identityColumn = null, SqlSugarClient con = null)
        {
            con ??= CreateConnection();
            var tran = await con.UseTranAsync(async () =>
            {
                var storageable = con.Storageable<T>(objs).Saveable();
                if (identityColumn != null) storageable.WhereColumns(identityColumn);
                var storage = storageable.ToStorage();
                if (storage.InsertList.Count > 0) await storage.AsInsertable.ExecuteCommandAsync();
                if (storage.UpdateList.Count > 0) await storage.AsUpdateable.ExecuteCommandAsync();
                string identity = null;
                object[] identities = null;
                if (identityColumn == null)
                {
                    var entityInfo = CreateConnection().EntityMaintenance.GetEntityInfo<T>();
                    var columnInfo = entityInfo.Columns.Where(item => item.IsIdentity).FirstOrDefault();
                    if (columnInfo != null)
                    {
                        identity = columnInfo.PropertyName;
                        identities = objs.Select(item => columnInfo.PropertyInfo.GetValue(item)).ToArray();
                    }
                }
                else
                {
                    identity = ((MemberExpression)identityColumn.Body).Member.Name;
                    identities = objs.Select(identityColumn.Compile()).ToArray();
                }
                if (identity == null || identities == null) return null;
                return await con.Queryable<T>().In(identity, identities).ToListAsync();
            }, ex => throw ex);
            return await tran.Data;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> expression = null, SqlSugarClient con = null)
        {
            con ??= CreateConnection();
            return await con.Deleteable<T>(expression).ExecuteCommandHasChangeAsync();
        }
    }
}
