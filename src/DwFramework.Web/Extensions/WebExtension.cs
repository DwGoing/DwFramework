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
using DwFramework.Web.WebApi;

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
        /// 配置WebApi
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi(this ServiceHost host, Config.Web config, Action<IWebHostBuilder> configureWebHostBuilder)
        {
            if (config == null) throw new Exception("未读取到WebApi配置");
            var webApiService = new WebApiService(host, config, configureWebHostBuilder);
            host.ConfigureContainer(builder => builder.RegisterInstance(webApiService).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi(this ServiceHost host, Config.Web config, Type startup)
            => host.ConfigureWebApi(config, builder => builder.UseStartup(startup));

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi<T>(this ServiceHost host, Config.Web config) where T : class
            => host.ConfigureWebApi(config, builder => builder.UseStartup<T>());

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="startup"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi(this ServiceHost host, IConfiguration configuration, Type startup, string path = null)
        {
            var config = configuration.GetConfig<Config.Web>(path);
            host.ConfigureWebApi(config, startup);
            return host;
        }

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi<T>(this ServiceHost host, IConfiguration configuration, string path = null) where T : class
        {
            var config = configuration.GetConfig<Config.Web>(path);
            host.ConfigureWebApi<T>(config);
            return host;
        }

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="startup"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithJson(this ServiceHost host, string file, Type startup, string path = null)
            => host.ConfigureWebApi(new ConfigurationBuilder().AddJsonFile(file).Build(), startup, path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithJson<T>(this ServiceHost host, string file, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddJsonFile(file).Build(), path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="startup"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithJson(this ServiceHost host, Stream stream, Type startup, string path = null)
            => host.ConfigureWebApi(new ConfigurationBuilder().AddJsonStream(stream).Build(), startup, path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithJson<T>(this ServiceHost host, Stream stream, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddJsonStream(stream).Build(), path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="startup"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithXml(this ServiceHost host, string file, Type startup, string path = null)
            => host.ConfigureWebApi(new ConfigurationBuilder().AddXmlFile(file).Build(), startup, path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithXml<T>(this ServiceHost host, string file, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddXmlFile(file).Build(), path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="startup"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithXml(this ServiceHost host, Stream stream, Type startup, string path = null)
            => host.ConfigureWebApi(new ConfigurationBuilder().AddXmlStream(stream).Build(), startup, path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithXml<T>(this ServiceHost host, Stream stream, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddXmlStream(stream).Build(), path);

        /// <summary>
        /// 获取WebApi服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="WebApiService"></typeparam>
        /// <returns></returns>
        public static WebApiService GetWebApi(this IServiceProvider provider) => provider.GetService<WebApiService>();

        /// <summary>
        /// 配置Web服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <param name="configureServices"></param>
        /// <param name="configure"></param>
        /// <param name="configureEndpoints"></param>
        /// <returns></returns>
        // public static ServiceHost ConfigureWeb(
        //     this ServiceHost host, Config.Web config,
        //     Action<IServiceCollection> configureServices = null,
        //     Action<IApplicationBuilder> configure = null,
        //     Action<IEndpointRouteBuilder> configureEndpoints = null
        // )
        // {
        //     if (config == null) throw new Exception("未读取到Web配置");
        //     var webService = new WebService(host, config, configureServices, configure, configureEndpoints);
        //     host.ConfigureContainer(builder => builder.RegisterInstance(webService).SingleInstance());
        //     return host;
        // }

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
        // public static ServiceHost ConfigureWeb(
        //     this ServiceHost host, IConfiguration configuration, string path = null,
        //     Action<IServiceCollection> configureServices = null,
        //     Action<IApplicationBuilder> configure = null,
        //     Action<IEndpointRouteBuilder> configureEndpoints = null
        // )
        // {
        //     var config = configuration.GetConfig<Config.Web>(path);
        //     host.ConfigureWeb(config, configureServices, configure, configureEndpoints);
        //     return host;
        // }

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
        // public static ServiceHost ConfigureWebWithJson(
        //     this ServiceHost host, string file, string path = null,
        //     Action<IServiceCollection> configureServices = null,
        //     Action<IApplicationBuilder> configure = null,
        //     Action<IEndpointRouteBuilder> configureEndpoints = null
        // )
        //     => host.ConfigureWeb(new ConfigurationBuilder().AddJsonFile(file).Build(), path, configureServices, configure, configureEndpoints);

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
        // public static ServiceHost ConfigureWebWithJson(
        //     this ServiceHost host, Stream stream, string path = null,
        //     Action<IServiceCollection> configureServices = null,
        //     Action<IApplicationBuilder> configure = null,
        //     Action<IEndpointRouteBuilder> configureEndpoints = null
        // )
        //     => host.ConfigureWeb(new ConfigurationBuilder().AddJsonStream(stream).Build(), path, configureServices, configure, configureEndpoints);

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
        // public static ServiceHost ConfigureWebWithXml(
        //     this ServiceHost host, string file, string path = null,
        //     Action<IServiceCollection> configureServices = null,
        //     Action<IApplicationBuilder> configure = null,
        //     Action<IEndpointRouteBuilder> configureEndpoints = null
        // )
        //     => host.ConfigureWeb(new ConfigurationBuilder().AddXmlFile(file).Build(), path, configureServices, configure, configureEndpoints);

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
        // public static ServiceHost ConfigureWebWithXml(
        //     this ServiceHost host, Stream stream, string path = null,
        //     Action<IServiceCollection> configureServices = null,
        //     Action<IApplicationBuilder> configure = null,
        //     Action<IEndpointRouteBuilder> configureEndpoints = null
        // )
        //     => host.ConfigureWeb(new ConfigurationBuilder().AddXmlStream(stream).Build(), path, configureServices, configure, configureEndpoints);

        /// <summary>
        /// 获取Web服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="WebApiService"></typeparam>
        /// <returns></returns>
        // public static WebService GetWeb(this IServiceProvider provider) => provider.GetService<WebService>();
    }
}
