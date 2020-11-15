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
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        public static void RegisterORMService(this ServiceHost host, string configKey = null, string configPath = null)
        {
            host.Register(c => new ORMService(configKey, configPath)).SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ORMService GetORMService(this IServiceProvider provider) => provider.GetService<ORMService>();
    }
}