using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace DwFramework.WebAPI.Plugins
{
    public static class RequestFilter
    {
        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="startHandler"></param>
        /// <param name="endHandler"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestFilter(this IApplicationBuilder app, Dictionary<string, Action<HttpContext>> handlers)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    foreach (var item in handlers)
                    {
                        if (Regex.Match(context.Request.Path, item.Key).Success)
                            item.Value.Invoke(context);
                    }
                    await next();
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
