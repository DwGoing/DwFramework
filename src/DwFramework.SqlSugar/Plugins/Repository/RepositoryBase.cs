using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using SqlSugar;

namespace DwFramework.SqlSugar.Repository
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class, new()
    {
        private readonly SqlSugarService _sqlSugarService;
        private readonly string _connName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlSugarService"></param>
        /// <param name="connName"></param>
        public RepositoryBase(SqlSugarService sqlSugarService, string connName)
        {
            _sqlSugarService = sqlSugarService;
            _connName = connName;
            if (_sqlSugarService == null) throw new Exception("sqlSugarService不能为空");
        }

        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="initKeyType"></param>
        /// <param name="entityNameService"></param>
        /// <param name="entityService"></param>
        /// <returns></returns>
        public SqlSugarClient CreateConnection(
            InitKeyType initKeyType = InitKeyType.Attribute,
            Action<Type, EntityInfo> entityNameService = null,
            Action<PropertyInfo, EntityColumnInfo> entityService = null
        ) => _sqlSugarService.CreateConnection(_connName, initKeyType, entityNameService, entityService);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ISugarQueryable<T> Select(SqlSugarClient conn, Expression<Func<T, bool>> expression = null)
        {
            var queryable = conn.Queryable<T>();
            if (expression != null) queryable = queryable.Where(expression);
            return queryable;
        }

        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="objs"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<List<T>> InsertOrUpdateAsync(SqlSugarClient conn, List<T> objs, Expression<Func<T, object>> identityColumn = null)
        {
            var storageable = conn.Storageable<T>(objs).Saveable();
            if (identityColumn != null) storageable.WhereColumns(identityColumn);
            var storage = storageable.ToStorage();
            if (storage.InsertList.Count > 0) await storage.AsInsertable.ExecuteCommandAsync();
            if (storage.UpdateList.Count > 0) await storage.AsUpdateable.ExecuteCommandAsync();
            string identity = null;
            object[] identities = null;
            if (identityColumn == null)
            {
                var entityInfo = _sqlSugarService.CreateConnection(_connName).EntityMaintenance.GetEntityInfo<T>();
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
            return conn.Queryable<T>().In(identity, identities).ToList();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(SqlSugarClient conn, Expression<Func<T, bool>> expression = null)
            => conn.Deleteable<T>(expression).ExecuteCommandHasChangeAsync();
    }
}
