using System;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Core.Cache
{
    public static class CacheExtension
    {
        /// <summary>
        /// 获取缓存服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ICache GetCache(this IServiceProvider provider) => provider.GetService<ICache>();
    }
}
