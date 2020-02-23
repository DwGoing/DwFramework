using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace DwFramework.Database
{
    public interface IRepository<T>
    {
        T[] FindAll();
        T[] Find(Expression<Func<T, bool>> expression);
        T FindSingle(Expression<Func<T, bool>> expression);
    }
}
