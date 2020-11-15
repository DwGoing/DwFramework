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
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetConfig<T>(this IConfiguration configuration, string path = null)
        {
            var section = configuration;
            if (!string.IsNullOrEmpty(path)) section = section.GetSection(path);
            return section.Get<T>();
        }
    }
}
