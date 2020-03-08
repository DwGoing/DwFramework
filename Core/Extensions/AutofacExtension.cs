using System;
using System.Linq;
using System.Collections.Generic;

using Autofac.Extensions.DependencyInjection;
using AutoFac.Extras.NLog.DotNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Core.Extensions
{
    public static class AutofacExtension
    {
        /// <summary>
        /// 服务是否注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool IsRegistered<T>(this IServiceProvider provider) where T : class
        {
            var services = provider.GetServices<T>();
            return services.Count() > 0;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetServices<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetServices(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// 注册NLog服务
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ServiceHost RegisterNLog(this ServiceHost host)
        {
            host.RegisterModule<NLogModule>();
            return host;
        }

        /// <summary>
        /// 注入上层服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseDwServiceProvider(this IWebHostBuilder host, IServiceProvider provider)
        {
            host.ConfigureServices(services =>
            {
                services.AddSingleton(new DwServiceProvider(provider as AutofacServiceProvider));
            });
            return host;
        }
    }
}
