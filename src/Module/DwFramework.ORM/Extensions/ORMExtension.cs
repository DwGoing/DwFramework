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
            host.RegisterType<ORMService>().SingleInstance();
            host.OnInitialized += provider => provider.ReconfigORMService(path, key);
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

        /// <summary>
        /// 重新加载配置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void ReconfigORMService(this IServiceProvider provider, string path = null, string key = null)
        {
            var service = provider.GetORMService();
            service.ReadConfig(path, key);
        }
    }
}