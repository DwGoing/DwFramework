using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace DwFramework.Web
{
    public static class RequestFilter
    {
        public static IApplicationBuilder UseRequestFilter(this IApplicationBuilder app, Action<HttpContext> startHandler, Action<HttpContext> endHandler)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    startHandler(context);
                    await next();
                    endHandler(context);
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync(ex.Message);
                }
            });
            return app;
        }
    }
}
