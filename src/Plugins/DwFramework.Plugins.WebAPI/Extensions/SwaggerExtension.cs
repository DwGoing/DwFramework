using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DwFramework.Plugins.WebAPI
{
    public static class SwaggerExtension
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
        /// <param name="description"></param>
        /// <param name="xmlPath"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, string name, string title, string version, string description = null, string xmlPath = null)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name, new OpenApiInfo()
                {
                    Title = title,
                    Version = version,
                    Description = description
                });
                if (xmlPath != null) c.IncludeXmlComments(xmlPath);
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
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, string name, string desc)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{name}/swagger.json", desc);
            });
            return app;
        }
    }
}
