using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Web.Extensions
{
    public enum WebType
    {
        Unknow = 0,
        Http = 1,
        WebSocket = 2,
        Socket = 3
    }

    public static class WebExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="type"></param>
        public static void RegisterWebService(this ServiceHost host, WebType type)
        {
            switch (type)
            {
                case WebType.Http:
                    host.RegisterType<HttpService>().SingleInstance();
                    break;
                case WebType.WebSocket:
                    host.RegisterType<WebSocketService>().SingleInstance();
                    break;
                case WebType.Socket:
                    host.RegisterType<SocketService>().SingleInstance();
                    break;
            }
        }

        /// <summary>
        /// 初始化Http服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitHttpServiceAsync<T>(this IServiceProvider provider) where T : class
        {
            return (provider.GetWebService(WebType.Http) as HttpService).OpenServiceAsync<T>();
        }

        /// <summary>
        /// 初始化WebSocket服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitWebSocketServiceAsync(this IServiceProvider provider)
        {
            return (provider.GetWebService(WebType.WebSocket) as WebSocketService).OpenServiceAsync();
        }

        /// <summary>
        /// 初始化Socket服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitSocketServiceAsync(this IServiceProvider provider)
        {
            return (provider.GetWebService(WebType.Socket) as SocketService).OpenServiceAsync();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BaseService GetWebService(this IServiceProvider provider, WebType type)
        {
            switch (type)
            {
                case WebType.Http:
                    return provider.GetService<HttpService>();
                case WebType.WebSocket:
                    return provider.GetService<WebSocketService>();
                case WebType.Socket:
                    return provider.GetService<SocketService>();
                default:
                    return null;
            }
        }
    }
}
