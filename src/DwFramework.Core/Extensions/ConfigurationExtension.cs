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
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(this IConfiguration configuration, string path = null)
            => configuration == null || string.IsNullOrEmpty(path) ? configuration : configuration.GetSection(path);

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ParseConfiguration<T>(this IConfiguration configuration, string path = null)
            => configuration.GetConfiguration(path).Get<T>();

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IConfiguration GetConfiguration(this IServiceProvider provider, string path = null)
            => provider.GetService<IConfiguration>().GetConfiguration(path);

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ParseConfiguration<T>(this IServiceProvider provider, string path = null)
            => provider.GetConfiguration(path).ParseConfiguration<T>();
    }
}
