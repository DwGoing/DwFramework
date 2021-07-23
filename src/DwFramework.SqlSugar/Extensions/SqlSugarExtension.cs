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
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugar(this ServiceHost host, string path = null)
            => host.ConfigureContainer(builder => builder.Register(_ => new SqlSugarService(ServiceHost.GetConfiguration(path))).SingleInstance());

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugar(this ServiceHost host, IConfiguration configuration, string path = null)
        {
            host.AddConfiguration(configuration);
            return host.ConfigureSqlSugar(path);
        }

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithJson(this ServiceHost host, string file, string path = null)
        {
            host.AddJsonConfiguration(file, reloadOnChange: true);
            return host.ConfigureSqlSugar(path);
        }

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithJson(this ServiceHost host, Stream stream, string path = null)
        {
            host.AddJsonConfiguration(stream);
            return host.ConfigureSqlSugar(path);
        }

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithXml(this ServiceHost host, string file, string path = null)
        {
            host.AddXmlConfiguration(file, reloadOnChange: true);
            return host.ConfigureSqlSugar(path);
        }

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureSqlSugarWithXml(this ServiceHost host, Stream stream, string path = null)
        {
            host.AddXmlConfiguration(stream);
            return host.ConfigureSqlSugar(path);
        }

        /// <summary>
        /// 获取SqlSugar
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static SqlSugarService GetSqlSugarService(this IServiceProvider provider) => provider.GetService<SqlSugarService>();
    }
}