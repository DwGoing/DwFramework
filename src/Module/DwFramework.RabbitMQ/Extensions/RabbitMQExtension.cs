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
        public static void RegisterRabbitMQService(this ServiceHost host)
        {
            host.RegisterType<RabbitMQService>().SingleInstance();
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
