using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using SqlSugar;

namespace DwFramework.Database
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class, new()
    {
        private DatabaseService _databaseService;

        public SqlSugarClient Db { get => _databaseService.Db; }

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
        /// <returns></returns>
        public Task<T[]> FindAllAsync()
        {
            return Task.Run(() => Db.Queryable<T>().ToArray());
        }

        /// <summary>
        /// 条件查找
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<T[]> FindAsync(Expression<Func<T, bool>> expression)
        {
            return Task.Run(() => Db.Queryable<T>().Where(expression).ToArray());
        }

        /// <summary>
        /// 查找一条记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<T> FindSingleAsync(Expression<Func<T, bool>> expression)
        {
            return Db.Queryable<T>().FirstAsync(expression);
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        public Task<T> InsertAsync(T newRecord)
        {
            return Db.Insertable(newRecord).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="newRecords"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(T[] newRecords)
        {
            return Db.Insertable(newRecords).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(Expression<Func<T, bool>> expression)
        {
            return Db.Deleteable(expression).ExecuteCommandAsync();
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="updatedRecord"></param>
        /// <returns></returns>
        public Task<bool> UpdateAsync(T updatedRecord)
        {
            return Db.Updateable(updatedRecord).ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="updatedRecords"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(T[] updatedRecords)
        {
            return Db.Updateable(updatedRecords).ExecuteCommandAsync();
        }
    }
}
