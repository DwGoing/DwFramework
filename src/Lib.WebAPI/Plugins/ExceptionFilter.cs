using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace DwFramework.WebAPI.Plugins
{
    public static class ExceptionFilter
    {
        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionFilter(this IApplicationBuilder app, Action<HttpContext, Exception> handler)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    handler(context, ex);
                }
            });
            return app;
        }
    }
}
