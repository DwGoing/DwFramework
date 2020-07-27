using System;
using System.Threading.Tasks;
using System.Linq;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Web.Extensions
{
    public static class WebExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterWebService<T>(this ServiceHost host) where T : class
        {
            RequireType(typeof(T));
            host.RegisterType<T>().SingleInstance();
        }

        /// <summary>
        /// 初始化Http服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitHttpServiceAsync<T>(this IServiceProvider provider) where T : class
        {
            var service = provider.GetWebService<HttpService>();
            if (service == null) throw new Exception("服务未注册");
            return service.OpenServiceAsync<T>();
        }

        /// <summary>
        /// 初始化WebSocket服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitWebSocketServiceAsync(this IServiceProvider provider)
        {
            var service = provider.GetWebService<WebSocketService>();
            if (service == null) throw new Exception("服务未注册");
            return service.OpenServiceAsync();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T GetWebService<T>(this IServiceProvider provider) where T : class
        {
            RequireType(typeof(T));
            return provider.GetService<T>();
        }

        /// <summary>
        /// 类型验证
        /// </summary>
        /// <param name="type"></param>
        private static void RequireType(Type type)
        {
            var services = new Type[] {
                typeof(HttpService),
                typeof(WebSocketService),
            };
            if (!services.Contains(type))
                throw new Exception("无法获取该类型的服务");
        }
    }
}
