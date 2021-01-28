using System;
using System.Threading.Tasks;

using DwFramework.Core;

namespace DwFramework.WebSocket
{
    public static class WebSocketExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterWebSocketService(this ServiceHost host, string path = null, string key = null)
        {
            host.Register(_ => new WebSocketService(path, key)).SingleInstance();
            host.OnInitializing += async provider => await provider.InitWebSocketServiceAsync();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static WebSocketService GetWebSocketService(this IServiceProvider provider)
        {
            return provider.GetService<WebSocketService>();
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        public static Task InitWebSocketServiceAsync(this IServiceProvider provider)
        {
            return provider.GetWebSocketService().OpenServiceAsync();
        }
    }
}
