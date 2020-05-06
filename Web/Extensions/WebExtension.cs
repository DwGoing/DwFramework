using System;
using System.Threading.Tasks;

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
        public static void RegisterWebService(this ServiceHost host)
        {
            host.RegisterType<WebService>().SingleInstance();
        }

        /// <summary>
        /// 初始化Http服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitHttpServiceAsync<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetWebService().OpenHttpServiceAsync<T>();
        }

        /// <summary>
        /// 初始化WebSocket服务
        /// </summary>
        /// <param name="provider"></param>
        public static Task InitWebSocketServiceAsync(this IServiceProvider provider, OnConnectHandler onConnect = null, OnSendHandler onSend = null, OnReceiveHandler onReceive = null, OnCloseHandler onClose = null, OnErrorHandler onError = null)
        {
            var service = provider.GetWebService();
            if (onConnect != null) service.OnConnect += onConnect;
            if (onSend != null) service.OnSend += onSend;
            if (onReceive != null) service.OnReceive += onReceive;
            if (onClose != null) service.OnClose += onClose;
            if (onError != null) service.OnError += onError;
            return service.OpenWebSocketServiceAsync();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static WebService GetWebService(this IServiceProvider provider)
        {
            return provider.GetService<WebService>();
        }
    }
}
