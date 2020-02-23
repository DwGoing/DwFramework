using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.RabbitMQ.Extensions
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
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitRabbitMQServiceAsync(this IServiceProvider provider)
        {
            return provider.GetService<RabbitMQService>().OpenServiceAsync();
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
