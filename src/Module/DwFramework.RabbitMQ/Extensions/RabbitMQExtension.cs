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
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        public static void RegisterRabbitMQService(this ServiceHost host, string configKey = null, string configPath = null)
        {
            host.Register(c => new RabbitMQService(configKey, configPath)).SingleInstance();
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
    }
}
