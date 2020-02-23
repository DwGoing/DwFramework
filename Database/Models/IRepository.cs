using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace DwFramework.Database
{
    public interface IRepository<T>
    {
        T FindSingle(Func<T, bool> expression);
        T[] Find(Func<T, bool> expression);
    }
}
