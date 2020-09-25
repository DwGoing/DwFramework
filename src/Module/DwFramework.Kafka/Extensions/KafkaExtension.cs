using System;
using Autofac;

using DwFramework.Core;

namespace DwFramework.Kafka
{
    public static class KafkaExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterDatabaseService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath);
                host.RegisterType<KafkaService>().SingleInstance();
            }
            else host.Register(c => new KafkaService(c.Resolve<Core.Environment>(), "Kafka")).SingleInstance();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static KafkaService GetDatabaseService(this IServiceProvider provider) => provider.GetService<KafkaService>();
    }
}
