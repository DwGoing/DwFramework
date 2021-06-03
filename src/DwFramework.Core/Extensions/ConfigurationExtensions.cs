using System;
using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetConfig<T>(this IConfiguration configuration, string path = null)
            => string.IsNullOrEmpty(path) ? configuration.Get<T>() : configuration.GetSection(path).Get<T>();
    }
}
