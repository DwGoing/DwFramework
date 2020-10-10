using System;
using Autofac;

using DwFramework.Core;

namespace DwFramework.RabbitMQ
{
    public static class RabbitMQExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterRabbitMQService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath, "RabbitMQ");
                host.RegisterType<RabbitMQService>().SingleInstance();
            }
            else host.Register(c => new RabbitMQService(c.Resolve<Core.Environment>(), "RabbitMQ")).SingleInstance();
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
