using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using DwFramework.Web;
using DwFramework.Web.Plugins;

namespace _Test.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // JWT插件
            services.AddJwtAuthentication(new JwtManager.DefaultJwtTokenValidator("fc3d06d9b75f92b648ab4e372dfd22f2"), context =>
            {
                Console.WriteLine("Success");
                return Task.CompletedTask;
            }, context =>
            {
                Console.WriteLine("Fail");
                return Task.CompletedTask;
            });
            services.AddControllers();
            services.AddSwagger("v1", "Test", "v1");
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseJwtAuthentication(); // 必须在UseRouting之后
            app.UseSwagger("/swagger/v1/swagger.json", "My API V1");
            app.UseRequestFilter(
                context => { },
                context => { });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}