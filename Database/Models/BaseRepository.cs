using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using SqlSugar;

namespace DwFramework.Database
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="databaseService"></param>
        public BaseRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            if (_databaseService == null)
                throw new Exception("未找到Database服务");
        }

        /// <summary>
        /// 查找所有记录
        /// </summary>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T[]> FindAllAsync(int cacheExpireSeconds = 0)
        {
            return Task.Run(() =>
            {
                return _databaseService.DbConnection.Queryable<T>()
                    .WithCacheIF(cacheExpireSeconds > 0, cacheExpireSeconds)
                    .ToArray();
            });
        }

        /// <summary>
        /// 条件查找
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T[]> FindAsync(Expression<Func<T, bool>> expression, int cacheExpireSeconds = 0)
        {
            return Task.Run(() =>
            {
                return _databaseService.DbConnection.Queryable<T>()
                    .Where(expression)
                    .WithCacheIF(cacheExpireSeconds > 0, cacheExpireSeconds)
                    .ToArray();
            });
        }

        /// <summary>
        /// 查找一条记录
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T> FindSingleAsync(Expression<Func<T, bool>> expression, int cacheExpireSeconds = 0)
        {
            return _databaseService.DbConnection.Queryable<T>()
                .WithCacheIF(cacheExpireSeconds > 0, cacheExpireSeconds)
                .FirstAsync(expression);
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        public Task<T> InsertAsync(T newRecord)
        {
            return _databaseService.DbConnection.Insertable(newRecord)
                .RemoveDataCache()
                .ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="newRecords"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(T[] newRecords)
        {
            return _databaseService.DbConnection.Insertable(newRecords)
                .RemoveDataCache()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(Expression<Func<T, bool>> expression)
        {
            return _databaseService.DbConnection.Deleteable(expression)
                .RemoveDataCache()
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="updatedRecord"></param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(T updatedRecord)
        {
            return _databaseService.DbConnection.Updateable(updatedRecord)
                .RemoveDataCache()
                .ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="updatedRecords"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(T[] updatedRecords)
        {
            return _databaseService.DbConnection.Updateable(updatedRecords)
                .RemoveDataCache()
                .ExecuteCommandAsync();
        }
    }
}
