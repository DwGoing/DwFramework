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
        /// <param name="configureServices"></param>
        /// <param name="configure"></param>
        /// <param name="configureEndpoints"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWeb(
            this ServiceHost host, Config.Web config,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Action<IEndpointRouteBuilder> configureEndpoints = null
        )
        {
            if (config == null) throw new Exception("未读取到Web配置");
            var webService = new WebService(host, config, configureServices, configure, configureEndpoints);
            host.ConfigureContainer(builder => builder.RegisterInstance(webService).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <param name="configureServices"></param>
        /// <param name="configure"></param>
        /// <param name="configureEndpoints"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWeb(
            this ServiceHost host, IConfiguration configuration, string path = null,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Action<IEndpointRouteBuilder> configureEndpoints = null
        )
        {
            var config = configuration.GetConfig<Config.Web>(path);
            host.ConfigureWeb(config, configureServices, configure, configureEndpoints);
            return host;
        }

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="configureServices"></param>
        /// <param name="configure"></param>
        /// <param name="configureEndpoints"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithJson(
            this ServiceHost host, string file, string path = null,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Action<IEndpointRouteBuilder> configureEndpoints = null
        )
            => host.ConfigureWeb(new ConfigurationBuilder().AddJsonFile(file).Build(), path, configureServices, configure, configureEndpoints);

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <param name="configureServices"></param>
        /// <param name="configure"></param>
        /// <param name="configureEndpoints"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithJson(
            this ServiceHost host, Stream stream, string path = null,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Action<IEndpointRouteBuilder> configureEndpoints = null
        )
            => host.ConfigureWeb(new ConfigurationBuilder().AddJsonStream(stream).Build(), path, configureServices, configure, configureEndpoints);

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="configureServices"></param>
        /// <param name="configure"></param>
        /// <param name="configureEndpoints"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithXml(
            this ServiceHost host, string file, string path = null,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Action<IEndpointRouteBuilder> configureEndpoints = null
        )
            => host.ConfigureWeb(new ConfigurationBuilder().AddXmlFile(file).Build(), path, configureServices, configure, configureEndpoints);

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <param name="configureServices"></param>
        /// <param name="configure"></param>
        /// <param name="configureEndpoints"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebWithXml(
            this ServiceHost host, Stream stream, string path = null,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Action<IEndpointRouteBuilder> configureEndpoints = null
        )
            => host.ConfigureWeb(new ConfigurationBuilder().AddXmlStream(stream).Build(), path, configureServices, configure, configureEndpoints);

        /// <summary>
        /// 获取Web服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="WebApiService"></typeparam>
        /// <returns></returns>
        public static WebService GetWeb(this IServiceProvider provider) => provider.GetService<WebService>();
    }
}
