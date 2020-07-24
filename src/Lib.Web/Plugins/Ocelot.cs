using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace DwFramework.Web.Plugins
{
    public static class OcelotManager
    {
        /// <summary>
        /// 注入Ocelot服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddOcelot(this IServiceCollection services)
        {
            services.AddOcelot();
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
