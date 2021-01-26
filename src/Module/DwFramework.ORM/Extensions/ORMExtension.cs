using System;

using DwFramework.Core;

namespace DwFramework.ORM
{
    public static class ORMExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterORMService(this ServiceHost host, string path = null, string key = null)
        {
            host.Register(_ => new ORMService(path, key)).SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ORMService GetORMService(this IServiceProvider provider)
        {
            return provider.GetService<ORMService>();
        }
    }
}