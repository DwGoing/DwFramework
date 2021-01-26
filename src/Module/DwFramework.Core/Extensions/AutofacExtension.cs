using System;
using System.Linq;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Core
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
            return provider.GetServices<T>().Any();
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
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static T GetService<T>(this ILifetimeScope scope) where T : class
        {
            return scope.Resolve<T>();
        }
    }
}