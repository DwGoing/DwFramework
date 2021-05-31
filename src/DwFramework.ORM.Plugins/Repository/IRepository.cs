using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using SqlSugar;

namespace DwFramework.ORM.Plugins
{
    public interface IRepository<T> where T : class, new()
    {
        public ISugarQueryable<T> Select(Expression<Func<T, bool>> expression = null, SqlSugarClient conn = null);
        public Task<List<T>> InsertOrUpdateAsync(List<T> objs, Expression<Func<T, object>> identity = null, SqlSugarClient con = null);
        public Task<bool> DeleteAsync(Expression<Func<T, bool>> expression = null, SqlSugarClient con = null);
    }
}