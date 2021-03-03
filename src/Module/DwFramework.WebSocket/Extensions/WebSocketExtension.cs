using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Autofac;
using DwFramework.Core;

namespace DwFramework.WebSocket
{
    public static class WebSocketExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        public static void RegisterWebSocketService(this ServiceHost host, string configKey = null, string configPath = null)
        {
            host.Register(c => new WebSocketService(configKey, configPath, c.Resolve<ILogger<WebSocketService>>())).SingleInstance();
            host.OnInitializing += async provider => await provider.InitWebSocketServiceAsync();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static WebSocketService GetWebSocketService(this IServiceProvider provider) => provider.GetService<WebSocketService>();

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        public static Task InitWebSocketServiceAsync(this IServiceProvider provider) => provider.GetWebSocketService().OpenServiceAsync();
    }
}
