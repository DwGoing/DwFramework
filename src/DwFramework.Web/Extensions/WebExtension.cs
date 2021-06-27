using System;
using System.IO;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.Web
{
    public static class WebExtension
    {
        /// <summary>
        /// 配置Web主机
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebHost(this ServiceHost host, Action<IWebHostBuilder> configure)
        {
            host.ConfigureHostBuilder(builder => builder.ConfigureWebHost(configure));
            return host;
        }

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWeb(this ServiceHost host, Config.Web config, Action<IWebHostBuilder> configureWebHostBuilder)
        {
            if (config == null) throw new Exception("未读取到Web配置");
            var webService = WebService.Init(host, config, configureWebHostBuilder);
            host.ConfigureContainer(builder => builder.RegisterInstance(webService).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWeb(this ServiceHost host, IConfiguration configuration, Action<IWebHostBuilder> configureWebHostBuilder, string path = null)
        {
            var config = configuration.GetConfig<Config.Web>(path);
            host.ConfigureWeb(config, configureWebHostBuilder);
            return host;
        }

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithJson(this ServiceHost host, string file, Action<IWebHostBuilder> configureWebHostBuilder, string path = null)
            => host.ConfigureWeb(new ConfigurationBuilder().AddJsonFile(file).Build(), configureWebHostBuilder, path);

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithJson(this ServiceHost host, Stream stream, Action<IWebHostBuilder> configureWebHostBuilder, string path = null)
            => host.ConfigureWeb(new ConfigurationBuilder().AddJsonStream(stream).Build(), configureWebHostBuilder, path);

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithXml(this ServiceHost host, string file, Action<IWebHostBuilder> configureWebHostBuilder, string path = null)
            => host.ConfigureWeb(new ConfigurationBuilder().AddXmlFile(file).Build(), configureWebHostBuilder, path);

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithXml(this ServiceHost host, Stream stream, Action<IWebHostBuilder> configureWebHostBuilder, string path = null)
            => host.ConfigureWeb(new ConfigurationBuilder().AddXmlStream(stream).Build(), configureWebHostBuilder, path);

        /// <summary>
        /// 获取Web服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="WebApiService"></typeparam>
        /// <returns></returns>
        public static WebService GetWeb(this IServiceProvider provider) => provider.GetService<WebService>();
    }
}
