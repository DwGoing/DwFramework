using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.WebAPI.Extensions
{
    public static class WebAPIExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterWebAPIService<T>(this ServiceHost host) where T : class
        {
            host.RegisterType<WebAPIService>().SingleInstance();
            host.OnInitializing += provider => provider.InitWebAPIServiceAsync<T>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static WebAPIService GetWebAPIService(this IServiceProvider provider)
        {
            return provider.GetService<WebAPIService>();
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitWebAPIServiceAsync<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetWebAPIService().OpenServiceAsync<T>();
        }
    }
}
