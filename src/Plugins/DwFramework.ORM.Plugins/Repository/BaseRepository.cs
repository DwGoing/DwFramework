using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.ORM.Plugins
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        private readonly ORMService _ormService;

        protected SqlSugarClient DbConnection { get => _ormService.DbConnection; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseRepository()
        {
            _ormService = ServiceHost.Provider.GetORMService();
            if (_ormService == null) throw new Exception("未找到ORM服务");
        }

        /// <summary>
        /// 查找所有记录
        /// </summary>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T[]> FindAllAsync(int cacheExpireSeconds = 0)
        {
            return TaskManager.CreateTask(() => DbConnection.Queryable<T>()
                    .WithCacheIF(cacheExpireSeconds > 0, cacheExpireSeconds)
                    .ToArray());
        }

        /// <summary>
        /// 查找所有记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T[]> FindAllAsync(int pageIndex, int pageSize, int cacheExpireSeconds = 0)
        {
            return TaskManager.CreateTask(() => DbConnection.Queryable<T>()
                    .WithCacheIF(cacheExpireSeconds > 0, cacheExpireSeconds)
                    .ToPageList(pageIndex, pageSize)
                    .ToArray());
        }

        /// <summary>
        /// 条件查找
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T[]> FindAsync(Expression<Func<T, bool>> expression, int cacheExpireSeconds = 0)
        {
            return TaskManager.CreateTask(() => DbConnection.Queryable<T>()
                    .Where(expression)
                    .WithCacheIF(cacheExpireSeconds > 0, cacheExpireSeconds)
                    .ToArray());
        }

        /// <summary>
        /// 条件查找
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T[]> FindAsync(Expression<Func<T, bool>> expression, int pageIndex, int pageSize, int cacheExpireSeconds = 0)
        {
            return TaskManager.CreateTask(() => DbConnection.Queryable<T>()
                    .Where(expression)
                    .ToPageList(pageIndex, pageSize)
                    .ToArray());
        }

        /// <summary>
        /// 查找一条记录
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cacheExpireSeconds"></param>
        /// <returns></returns>
        public Task<T> FindSingleAsync(Expression<Func<T, bool>> expression, int cacheExpireSeconds = 0)
        {
            return DbConnection.Queryable<T>()
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
            return DbConnection.Insertable(newRecord)
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
            return DbConnection.Insertable(newRecords)
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
            return DbConnection.Deleteable(expression)
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
            return DbConnection.Updateable(updatedRecord)
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
            return DbConnection.Updateable(updatedRecords)
                .RemoveDataCache()
                .ExecuteCommandAsync();
        }
    }
}
