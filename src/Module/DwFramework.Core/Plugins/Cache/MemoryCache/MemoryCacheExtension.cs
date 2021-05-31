using System;
using Autofac;

namespace DwFramework.Core.Plugins
{
    public static class MemoryCacheExtension
    {
        /// <summary>
        /// 注册MemoryCache服务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="storeCount"></param>
        /// <param name="isGlobal"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterMemoryCache(this ContainerBuilder builder, int storeCount = 6, bool isGlobal = true)
        {
            var registration = builder.Register(context => new MemoryCache(storeCount)).As<ICache>();
            if (isGlobal) registration.SingleInstance();
            return builder;
        }
    }
}
