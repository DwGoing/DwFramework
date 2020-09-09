using Microsoft.Extensions.Configuration;

namespace DwFramework.Core
{
    public static class ConfigurationExtension
    {
        /// <summary>
        /// 从根节点获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static T GetRoot<T>(this IConfiguration configuration) => configuration.Get<T>();

        /// <summary>
        /// 从子节点获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetConfig<T>(this IConfiguration configuration, string key) => configuration.GetSection(key).Get<T>();
    }
}
