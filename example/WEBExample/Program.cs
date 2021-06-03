using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DwFramework.Core;
using DwFramework.WEB;
using DwFramework.WEB.Plugins;

namespace WEBExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureWebApiWithJson<Startup>("Config.json", "WebApi");
            host.ConfigureLogging(builder => builder.UserNLog());
            await host.RunAsync();
        }
    }

    public sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
            services.AddControllers(options =>
            {
                // options.Filters.Add<ExceptionFilter>();
            }).AddJsonOptions(options =>
            {
                //不使用驼峰样式的key
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //不使用驼峰样式的key
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.UseCors("any");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
