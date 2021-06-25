using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.Web.WebApi
{
    public static class WebApiExtension
    {
        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi<T>(this ServiceHost host, Config config) where T : class
        {
            if (config == null) throw new Exception("未读取到WebApi配置");
            var webApiService = new WebApiService(host, config, typeof(T));
            host.ConfigureContainer(builder => builder.RegisterInstance(webApiService).SingleInstance());
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
            var config = configuration.GetConfig<Config>(path);
            host.ConfigureWebApi<T>(config);
            return host;
        }

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
    }
}
