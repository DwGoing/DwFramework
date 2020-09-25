using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public static class ConfigurationExtension
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetConfig<T>(this IConfiguration configuration, string key = null)
        {
            var section = configuration;
            if (!string.IsNullOrEmpty(key)) section = section.GetSection(key);
            return section.Get<T>();
        }
    }
}
