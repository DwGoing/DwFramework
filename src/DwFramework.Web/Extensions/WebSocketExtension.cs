using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.Web.WebSocket
{
    public static class WebSocketExtension
    {
        /// <summary>
        /// 配置WebSocket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebSocket(this ServiceHost host, Config config)
        {
            if (config == null) throw new Exception("未读取到WebSocket配置");
            var webSocketService = new WebSocketService(host, config);
            host.ConfigureContainer(builder => builder.RegisterInstance(webSocketService).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置WebSocket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebSocket(this ServiceHost host, IConfiguration configuration, string path = null)
        {
            var config = configuration.GetConfig<Config>(path);
            host.ConfigureWebSocket(config);
            return host;
        }

        /// <summary>
        /// 配置WebSocket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebSocketWithJson(this ServiceHost host, string file, string path = null)
            => host.ConfigureWebSocket(new ConfigurationBuilder().AddJsonFile(file).Build(), path);

        /// <summary>
        /// 配置WebSocket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebSocketWithJson(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureWebSocket(new ConfigurationBuilder().AddJsonStream(stream).Build(), path);

        /// <summary>
        /// 配置WebSocket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebSocketWithXml(this ServiceHost host, string file, string path = null)
            => host.ConfigureWebSocket(new ConfigurationBuilder().AddXmlFile(file).Build(), path);

        /// <summary>
        /// 配置WebSocket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebSocketWithXml(this ServiceHost host, Stream stream, string path = null)
            => host.ConfigureWebSocket(new ConfigurationBuilder().AddXmlStream(stream).Build(), path);

        /// <summary>
        /// 获取WebSocket服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="WebSocketService"></typeparam>
        /// <returns></returns>
        public static WebSocketService GetWebSocket(this IServiceProvider provider) => provider.GetService<WebSocketService>();
    }
}
