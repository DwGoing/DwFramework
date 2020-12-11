using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace DwFramework.WebAPI.Plugins
{
    public static class RequestFilterExtension
    {
        /// <summary>
        /// 获取当前RequestId
        /// </summary>
        /// <returns></returns>
        public static string GetRequestId() => Trace.CorrelationManager.ActivityId.ToString();

        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="exceptionHandler"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestFilter(this IApplicationBuilder app, Action<HttpContext, Exception> exceptionHandler)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    await next();
                }
                catch (Exception ex)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    exceptionHandler?.Invoke(context, ex);
                }
            });
            return app;
        }

        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="startHandler"></param>
        /// <param name="endHandler"></param>
        /// <param name="exceptionHandler"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestFilter(this IApplicationBuilder app, Action<HttpContext> startHandler, Action<HttpContext> endHandler, Action<HttpContext, Exception> exceptionHandler = null)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    startHandler(context);
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    await next();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    endHandler(context);
                }
                catch (Exception ex)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    exceptionHandler?.Invoke(context, ex);
                }
            });
            return app;
        }

        /// <summary>
        /// 注册过滤器
        /// </summary>
        /// <param name="app"></param>
        /// <param name="handlers"></param>
        /// <param name="exceptionHandler"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestFilter(this IApplicationBuilder app, Dictionary<string, Action<HttpContext>> handlers, Action<HttpContext, Exception> exceptionHandler = null)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    foreach (var item in handlers)
                    {
                        if (Regex.Match(context.Request.Path, item.Key).Success)
                        {
                            context.Request.Body.Seek(0, SeekOrigin.Begin);
                            item.Value.Invoke(context);
                        }
                    }
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    await next();
                }
                catch (Exception ex)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    exceptionHandler?.Invoke(context, ex);
                }
            });
            return app;
        }
    }
}
