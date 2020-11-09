namespace DwFramework.Core
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

    /// <summary>
    /// 压缩类型
    /// </summary>
    public enum CompressType
    {
        Unknow = 0,
        Brotli = 1,
        GZip = 2,
        LZ4 = 3
    }
}
