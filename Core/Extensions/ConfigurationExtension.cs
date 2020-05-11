using Microsoft.Extensions.Configuration;

namespace DwFramework.Core.Extensions
{
    public static class ConfigurationExtension
    {
        public static T GetConfig<T>(this IConfiguration configuration, string key)
        {
            return configuration.GetSection(key).Get<T>();
        }
    }
}
