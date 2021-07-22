using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.Core
{
    public static class ConfigurationExtension
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ParseConfiguration<T>(this IServiceProvider provider, string path = null)
        {
            var root = provider.GetService<IConfiguration>();
            if (root == null) return default;
            return string.IsNullOrEmpty(path) ? root.Get<T>() : root.GetSection(path).Get<T>();
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T ParseConfiguration<T>(this IConfiguration configuration, string path = null)
            => string.IsNullOrEmpty(path) ? configuration.Get<T>() : configuration.GetSection(path).Get<T>();
    }
}
