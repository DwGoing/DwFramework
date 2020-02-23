using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace DwFramework.Database
{
    public interface IRepository<T>
    {
        T[] Find(Func<T, bool> expression);
        T FindSingle(Func<T, bool> expression);
    }
}
