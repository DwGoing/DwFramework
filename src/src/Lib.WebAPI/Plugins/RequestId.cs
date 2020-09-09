using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Builder;

namespace DwFramework.WebAPI.Plugins
{
    public static class RequestId
    {
        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestId(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                await next();
            });
            return app;
        }

        /// <summary>
        /// 获取当前RequestId
        /// </summary>
        /// <returns></returns>
        public static string Get()
        {
            return Trace.CorrelationManager.ActivityId.ToString();
        }
    }
}
