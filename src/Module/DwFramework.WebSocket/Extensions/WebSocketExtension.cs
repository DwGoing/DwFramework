using System;
using System.Threading.Tasks;
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
        /// <param name="configFilePath"></param>
        public static void RegisterWebSocketService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath);
                host.RegisterType<WebSocketService>().SingleInstance();
            }
            else host.Register(c => new WebSocketService(c.Resolve<Core.Environment>(), "WebSocket")).SingleInstance();
            host.OnInitializing += provider => provider.InitWebSocketServiceAsync();
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
