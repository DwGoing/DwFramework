using System;

namespace DwFramework.Database
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        protected readonly DatabaseService _databaseService;

        public RepositoryBase(IDatabaseService databaseService)
        {
            _databaseService = (DatabaseService)databaseService;
        }

        public abstract T[] Find(Func<T, bool> expression);

        public abstract T FindSingle(Func<T, bool> expression);
    }
}
