using System;

namespace DwFramework.Core
{
    /// <summary>
    /// 环境类型
    /// </summary>
    public enum EnvironmentType
    {
        Development = 0,
        Production = 1
    }

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
