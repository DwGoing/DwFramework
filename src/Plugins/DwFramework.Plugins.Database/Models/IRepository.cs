using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DwFramework.Database.Repository
{
    public interface IRepository<T> where T : class, new()
    {
        Task<T[]> FindAllAsync(int cacheExpireSeconds = 0);
        Task<T[]> FindAllAsync(int pageIndex, int pageSize, int cacheExpireSeconds = 0);
        Task<T[]> FindAsync(Expression<Func<T, bool>> expression, int cacheExpireSeconds = 0);
        Task<T[]> FindAsync(Expression<Func<T, bool>> expression, int pageIndex, int pageSize, int cacheExpireSeconds = 0);
        Task<T> FindSingleAsync(Expression<Func<T, bool>> expression, int cacheExpireSeconds = 0);
        Task<T> InsertAsync(T newRecord);
        Task<int> InsertAsync(T[] newRecords);
        Task<int> DeleteAsync(Expression<Func<T, bool>> expression);
        Task<bool> UpdateAsync(T newRecord);
        Task<int> UpdateAsync(T[] newRecords);
    }
}