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
        return provider.GetService<WebService>().OpenHttpServiceAsync<T>();
    }

    /// <summary>
    /// 初始化WebSocket服务
    /// </summary>
    /// <param name="provider"></param>
    public static Task InitWebSocketServiceAsync<T>(this IServiceProvider provider)
    {
        return provider.GetService<WebService>().OpenWebSocketServiceAsync();
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
