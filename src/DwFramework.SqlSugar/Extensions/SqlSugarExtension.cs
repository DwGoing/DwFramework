using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.SqlSugar
{
    public static class SqlSugarExtension
    {
        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugar(this ServiceHost host, Config config)
        {
            if (config == null) throw new Exception("未读取到ORM配置");
            host.ConfigureContainer(builder => builder.Register(_ => new SqlSugarService(config)).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugar(this ServiceHost host, IConfiguration configuration, string path = null)
            => host.ConfigureSqlSugar(configuration.GetConfig<Config>(path));

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithJson(this ServiceHost host, string file, string path = null)
            => host.ConfigureSqlSugar(new ConfigurationBuilder().AddJsonFile(file).Build(), path);

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithJson(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureSqlSugar(new ConfigurationBuilder().AddJsonStream(stream).Build(), path);

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithXml(this ServiceHost host, string file, string path = null)
            => host.ConfigureSqlSugar(new ConfigurationBuilder().AddXmlFile(file).Build(), path);

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithXml(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureSqlSugar(new ConfigurationBuilder().AddXmlStream(stream).Build(), path);

        /// <summary>
        /// 获取SqlSugar服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static SqlSugarService GetSqlSugarService(this IServiceProvider provider) => provider.GetService<SqlSugarService>();
    }
}