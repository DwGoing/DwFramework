using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using DwFramework.Core;
using DwFramework.Web;

namespace WebExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureWebApiWithJson<Startup>("Config.json");
            host.ConfigureWebSocketWithJson("Config.json");
            host.ConfigureSocketWithJson("Config.json");
            host.ConfigureLogging(builder => builder.UserNLog());
            host.OnHostStarted += p =>
            {
                var w = p.GetSocket();
                w.OnConnect += (c, a) => Console.WriteLine($"{c.ID} connected");
                w.OnReceive += (c, a) =>
                {
                    Console.WriteLine($"{c.ID} received {Encoding.UTF8.GetString(a.Data)}");
                    var body = @"<h1>Hello World</h1>";
                    _ = c.SendAsync(Encoding.UTF8.GetBytes(
                        "HTTP/1.1 200 OK\r\n"
                        + "Date: Sat, 31 Dec 2005 23:59:59 GMT\r\n"
                        + "Content-Type: text/html;charset=UTF8\r\n"
                        + $"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n\n"
                        + $"{body}"
                    ));
                };
                w.OnSend += (c, a) => Console.WriteLine($"{c.ID} sent {Encoding.UTF8.GetString(a.Data)}");
                w.OnClose += (c, a) => Console.WriteLine($"{c.ID} closed");
            };
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("name", new OpenApiInfo()
                {
                    Title = "title",
                    Version = "version",
                    Description = "description"
                });
            });
            services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();
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
            app.UseSwagger(c => c.RouteTemplate = "{documentName}/swagger.json");
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/{"name"}/swagger.json", "desc"));
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
