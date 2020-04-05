using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DwFramework.Web.Plugins
{
    public static class SwaggerManager
    {
        /// <summary>
        /// 注入Swagger服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, string name, OpenApiInfo info)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name, info);
            });
            return services;
        }

        /// <summary>
        /// 注入Swagger服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, string name, string title, string version, string description = null)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name, new OpenApiInfo()
                {
                    Title = title,
                    Version = version,
                    Description = description
                });
            });
            return services;
        }

        /// <summary>
        /// 使用Swagger服务
        /// </summary>
        /// <param name="app"></param>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, string url, string name)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url, name);
            });
            return app;
        }
    }
}
