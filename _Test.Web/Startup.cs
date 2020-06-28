using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using DwFramework.Core.Plugins;
using DwFramework.Web;
using DwFramework.Web.Plugins;

namespace _Test.Web
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger)
        {
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwagger("Doc", "Test", "v1");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            //app.UseJwtAuthentication(); // 必须在UseRouting之后
            app.UseSwagger("Doc", "My API V1");
            app.UseRequestFilter(new Dictionary<string, Action<HttpContext>>
            {
                {"/*",context =>{
                    // 请求日志
                    Console.WriteLine($"接收到请求:{context.Request.Path} ({GetIP(context)})");
                    //_logger.LogInformation($"接收到请求:{context.Request.Path} ({GetIP(context)})");
                }}
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private string GetIP(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
                ip = IPAddress.Parse(ip).MapToIPv4().ToString();
            if (string.IsNullOrEmpty(ip))
                ip = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            return ip;
        }
    }
}