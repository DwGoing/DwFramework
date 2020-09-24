using System;
using System.Threading.Tasks;
using Autofac;

using DwFramework.Core;

namespace DwFramework.WebAPI
{
    public static class WebAPIExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterWebAPIService<T>(this ServiceHost host, string configFilePath = null) where T : class
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath);
                host.RegisterType<WebAPIService>().SingleInstance();
            }
            else host.Register(c => new WebAPIService(c.Resolve<Core.Environment>(), "WebAPI")).SingleInstance();
            host.RegisterType<WebAPIService>().SingleInstance();
            host.OnInitializing += provider => provider.InitWebAPIServiceAsync<T>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static WebAPIService GetWebAPIService(this IServiceProvider provider) => provider.GetService<WebAPIService>();

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitWebAPIServiceAsync<T>(this IServiceProvider provider) where T : class => provider.GetWebAPIService().OpenServiceAsync<T>();
    }
}
