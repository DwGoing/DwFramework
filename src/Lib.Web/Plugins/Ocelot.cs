using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

using DwFramework.Core;
using Ocelot.Configuration.File;

namespace DwFramework.Web.Plugins
{
    public static class OcelotManager
    {
        /// <summary>
        /// 注入Ocelot服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public static IServiceCollection AddOcelot(this IServiceCollection services, string configFilePath)
        {
            services.AddOcelot(ServiceHost.Environment.LoadConfiguration("Ocelot", configFilePath));
            return services;
        }

        /// <summary>
        /// 使用Ocelot服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOcelot(this IApplicationBuilder app)
        {
            app.UseOcelot();
            return app;
        }
    }
}
