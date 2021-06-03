using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DwFramework.WebAPI
{
    public static class DefaultOperationMiddlewareExtension
    {
        public static IApplicationBuilder UseDefaultOperation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DefaultOperationMiddleware>();
        }
    }

    public class DefaultOperationMiddleware
    {
        private readonly RequestDelegate _next;

        public DefaultOperationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 请求ID
            Trace.CorrelationManager.ActivityId = Guid.NewGuid();
            Console.WriteLine(1);
            await _next(context);
        }
    }
}
