using System;
using System.Linq.Expressions;

namespace DwFramework.Database
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        protected readonly DatabaseService _databaseService;

        public RepositoryBase(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public T[] FindAll()
        {
            return _databaseService.Db.Queryable<T>().ToArray();
        }

        public T[] Find(Expression<Func<T, bool>> expression)
        {
            return _databaseService.Db.Queryable<T>().Where(expression).ToArray();
        }

        public T FindSingle(Expression<Func<T, bool>> expression)
        {
            return _databaseService.Db.Queryable<T>().Where(expression).Single();
        }
    }
}
