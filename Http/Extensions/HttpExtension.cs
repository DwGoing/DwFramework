using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Http.Extensions
{
    public static class HttpExtension
    {
        /// <summary>
        /// 注册Http服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterHttpService(this ServiceHost host)
        {
            host.RegisterType<IHttpService, HttpService>().SingleInstance();
        }

        /// <summary>
        /// 初始化Http服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitHttpServiceAsync<T>(this IServiceProvider provider) where T : class, IHttpStartup
        {
            return provider.GetService<HttpService>().OpenServiceAsync<T>();
        }
    }
}
