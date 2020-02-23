using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Http.Extensions
{
    public static class HttpExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterHttpService(this ServiceHost host)
        {
            host.RegisterType<HttpService>().SingleInstance();
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitHttpServiceAsync<T>(this IServiceProvider provider) where T : class, IHttpStartup
        {
            return provider.GetService<HttpService>().OpenServiceAsync<T>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static HttpService GetHttpService(this IServiceProvider provider)
        {
            return provider.GetService<HttpService>();
        }
    }
}
