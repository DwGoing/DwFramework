namespace DwFramework.Core.Enums
{
    /// <summary>
    /// 环境类型
    /// </summary>
    public enum EnvironmentType
    {
        Develop = 0,
        Release = 1
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
