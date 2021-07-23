using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.Web
{
    public static class WebExtension
    {
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
            var webService = WebService.Init(host, configuration.GetConfiguration(path), configureWebHostBuilder);
            host.ConfigureContainer(builder => builder.RegisterInstance(webService).SingleInstance());
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

        /// <summary>
        /// 添加Rpc服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRpcImplements(this IServiceCollection services, params Type[] rpcImpls)
        {
            WebService.Instance.AddRpcImplements(services, rpcImpls);
            return services;
        }

        /// <summary>
        /// 匹配Rpc路由
        /// </summary>
        /// <param name="endpoints"></param>
        /// <returns></returns>
        public static IEndpointRouteBuilder MapRpcImplements(this IEndpointRouteBuilder endpoints)
        {
            WebService.Instance.MapRpcImplements(endpoints);
            return endpoints;
        }

        /// <summary>
        /// 使用WebSocket中间件
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWebSocket(this IApplicationBuilder app)
        {
            WebService.Instance.UseWebSocket(app);
            return app;
        }
    }
}
