using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.WEB.Plugins
{
    public static class VersionExtension
    {
        /// <summary>
        /// 使用API版本控制
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApiVersion(this IServiceCollection services, ApiVersion defaultVersion)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = defaultVersion;
            });
            return services;
        }
    }
}
