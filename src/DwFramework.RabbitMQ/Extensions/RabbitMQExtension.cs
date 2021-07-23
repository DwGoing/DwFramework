using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.RabbitMQ
{
    public static class RabbitMQExtension
    {
        /// <summary>
        /// 配置RabbitMQ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRabbitMQ(this ServiceHost host, IConfiguration configuration, string path = null)
            => host.ConfigureContainer(builder => builder.Register(_ => new RabbitMQService(configuration.GetConfiguration(path))).SingleInstance());

        /// <summary>
        /// 配置RabbitMQ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRabbitMQWithJson(this ServiceHost host, string file, string path = null)
            => host.ConfigureRabbitMQ(new ConfigurationBuilder().AddJsonFile(file).Build(), path);

        /// <summary>
        /// 配置RabbitMQ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRabbitMQWithJson(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureRabbitMQ(new ConfigurationBuilder().AddJsonStream(stream).Build(), path);

        /// <summary>
        /// 配置RabbitMQ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRabbitMQWithXml(this ServiceHost host, string file, string path = null)
            => host.ConfigureRabbitMQ(new ConfigurationBuilder().AddXmlFile(file).Build(), path);

        /// <summary>
        /// 配置RabbitMQ
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRabbitMQWithXml(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureRabbitMQ(new ConfigurationBuilder().AddXmlStream(stream).Build(), path);

        /// <summary>
        /// 获取RabbitMQ服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static RabbitMQService GetRabbitMQ(this IServiceProvider provider) => provider.GetService<RabbitMQService>();
    }
}
