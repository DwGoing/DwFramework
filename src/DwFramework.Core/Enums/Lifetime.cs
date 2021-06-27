using System;

namespace DwFramework.Core
{
    /// <summary>
    /// 生命周期
    /// </summary>
    public enum Lifetime
    {
        InstancePerDependency,
        Singleton,
        InstancePerLifetimeScope
    }
}
