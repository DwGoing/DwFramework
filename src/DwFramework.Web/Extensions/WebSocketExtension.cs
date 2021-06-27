using System;
using Microsoft.AspNetCore.Builder;

namespace DwFramework.Web.WebSocket
{
    public static class WebSocketExtension
    {
        /// <summary>
        /// 使用WebSocket中间件
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseWebSocket(this IApplicationBuilder app)
        {
            WebService.Instance.UseWebSocket(app);
            return app;
        }
    }
}