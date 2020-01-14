using System;
using System.Linq.Expressions;

namespace DwFramework.Database
{
    public interface IDatabaseService
    {
        T[] QueryWithCache<T>(bool forceFromDatabase = false);
        T[] QueryWithCache<T>(Expression<Func<T, bool>> expression, bool forceFromDatabase = false);
    }
}
