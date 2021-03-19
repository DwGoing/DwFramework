using System;

using DwFramework.Core;

namespace DwFramework.RabbitMQ
{
    public static class RabbitMQExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public static void RegisterRabbitMQService(this ServiceHost host, RabbitMQService.Config config)
        {
            host.RegisterType<RabbitMQService>().SingleInstance();
            host.OnInitialized += provider => provider.ConfigRabbitMQService(config);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterRabbitMQService(this ServiceHost host, string path = null, string key = null)
        {
            host.RegisterType<RabbitMQService>().SingleInstance();
            host.OnInitialized += provider => provider.ConfigRabbitMQService(path, key);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static RabbitMQService GetRabbitMQService(this IServiceProvider provider)
        {
            return provider.GetService<RabbitMQService>();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void ConfigRabbitMQService(this IServiceProvider provider, RabbitMQService.Config config)
        {
            var service = provider.GetRabbitMQService();
            service.ReadConfig(config);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void ConfigRabbitMQService(this IServiceProvider provider, string path = null, string key = null)
        {
            var service = provider.GetRabbitMQService();
            service.ReadConfig(path, key);
        }
    }
}
