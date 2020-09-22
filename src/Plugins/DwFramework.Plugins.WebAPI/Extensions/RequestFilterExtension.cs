using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace DwFramework.WebAPI.RequestFilter
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
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                    await next();
                }
                catch (Exception ex)
                {
                    exceptionHandler(context, ex);
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
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                    startHandler(context);
                    await next();
                    endHandler(context);
                }
                catch (Exception ex)
                {
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
                    Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                    foreach (var item in handlers)
                    {
                        if (Regex.Match(context.Request.Path, item.Key).Success)
                            item.Value.Invoke(context);
                    }
                    await next();
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(context, ex);
                }
            });
            return app;
        }
    }
}
