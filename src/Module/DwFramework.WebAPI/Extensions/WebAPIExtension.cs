using System;
using System.Threading.Tasks;

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
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterWebAPIService<T>(this ServiceHost host, string path = null, string key = null) where T : class
        {
            host.RegisterType<WebAPIService>().SingleInstance();
            host.OnInitialized += async provider => await provider.RunWebAPIServiceAsync<T>(path, key);
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
        /// 运行服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static async Task RunWebAPIServiceAsync<T>(this IServiceProvider provider, string path = null, string key = null) where T : class
        {
            var service = provider.GetWebAPIService();
            service.ReadConfig(path, key);
            await service.RunAsync<T>();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="provider"></param>
        public static void StopWebAPIService(this IServiceProvider provider)
        {
            var service = provider.GetWebAPIService();
            service.Stop();
        }
    }
}
