using Autofac;

namespace DwFramework.Core.Cache
{
    public static class MemoryCacheExtension
    {
        /// <summary>
        /// 注册MemoryCache服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="storeCount"></param>
        /// <param name="isGlobal"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureMemoryCache(this ServiceHost host, int storeCount = 6, bool isGlobal = true)
        {
            return host.ConfigureContainer(builder =>
            {
                var registration = builder.Register(context => new MemoryCache(storeCount)).As<ICache>();
                if (isGlobal) registration.SingleInstance();
            });
        }
    }
}
