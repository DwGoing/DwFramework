using Microsoft.Extensions.Configuration;

namespace DwFramework.Core.Extensions
{
    public static class ConfigurationExtension
    {
        public static T GetConfig<T>(this IConfiguration configuration, string key)
        {
            return configuration.GetSection(key).Get<T>();
        }

        public static void SetConfig(this IConfiguration configuration, string key, object value)
        {
            configuration.GetSection(key).Value = value.ToJson();
        }

        public static void SaveConfig(this IConfiguration configuration, string path)
        {
        }
    }
}
