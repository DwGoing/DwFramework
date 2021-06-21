using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.ORM
{
    public static class ORMExtension
    {
        /// <summary>
        /// 配置ORM
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureORM(this ServiceHost host, Config config)
        {
            if (config == null) throw new Exception("未读取到ORM配置");
            host.ConfigureContainer(builder => builder.Register(_ => new ORMService(config)).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置ORM
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureORM(this ServiceHost host, IConfiguration configuration, string path = null)
            => host.ConfigureORM(configuration.GetConfig<Config>(path));

        /// <summary>
        /// 配置ORM
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureORMWithJson(this ServiceHost host, string file, string path = null)
            => host.ConfigureORM(new ConfigurationBuilder().AddJsonFile(file).Build(), path);

        /// <summary>
        /// 配置ORM
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureORMWithJson(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureORM(new ConfigurationBuilder().AddJsonStream(stream).Build(), path);

        /// <summary>
        /// 配置ORM
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureORMWithXml(this ServiceHost host, string file, string path = null)
            => host.ConfigureORM(new ConfigurationBuilder().AddXmlFile(file).Build(), path);

        /// <summary>
        /// 配置ORM
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureORMWithXml(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureORM(new ConfigurationBuilder().AddXmlStream(stream).Build(), path);

        /// <summary>
        /// 获取ORM服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ORMService GetORMService(this IServiceProvider provider) => provider.GetService<ORMService>();
    }
}