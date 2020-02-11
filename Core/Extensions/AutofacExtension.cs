using System;
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
        /// 获取服务
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T GetService<I, T>(this IServiceProvider provider) where T : class where I : class
        {
            var services = provider.GetServices<I>();
            foreach (var item in services)
            {
                if (item.GetType() == typeof(T))
                    return item as T;
            }
            return default;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IEnumerable<I> GetAllServices<I>(this IServiceProvider provider) where I : class
        {
            return provider.GetServices<I>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<object> GetAllServices(this IServiceProvider provider, Type type)
        {
            return provider.GetServices(type);
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
        /// 注入在上层注入的服务
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
