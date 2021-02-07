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
            host.RegisterType<WebSocketService>().SingleInstance();
            host.OnInitialized += async provider => await provider.RunWebSocketServiceAsync(path, key);
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
        /// 运行服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task RunWebSocketServiceAsync(this IServiceProvider provider, string path = null, string key = null)
        {
            var service = provider.GetWebSocketService();
            service.ReadConfig(path, key);
            await service.RunAsync();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="provider"></param>
        public static void StopWebSocketService(this IServiceProvider provider)
        {
            var service = provider.GetWebSocketService();
            service.Stop();
        }
    }
}
