using System;
using System.Threading.Tasks;
using System.Linq;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.WebSocket.Extensions
{
    public static class WebSocketExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterWebSocketService(this ServiceHost host)
        {
            host.RegisterType<WebSocketService>().SingleInstance();
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
