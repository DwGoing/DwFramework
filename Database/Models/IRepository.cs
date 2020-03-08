using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DwFramework.Database
{
    public interface IRepository<T> where T : class, new()
    {
        Task<T[]> FindAllAsync();
        Task<T[]> FindAsync(Expression<Func<T, bool>> expression);
        Task<T> FindSingleAsync(Expression<Func<T, bool>> expression);
        Task<T> InsertAsync(T newRecord);
        Task<int> InsertAsync(T[] newRecords);
        Task<int> DeleteAsync(Expression<Func<T, bool>> expression);
        Task<bool> UpdateAsync(T newRecord);
        Task<int> UpdateAsync(T[] newRecords);
    }
}