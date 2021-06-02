using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using DwFramework.Core;
using DwFramework.WEB;

namespace CoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.AddJsonConfig("X.json");
            host.ConfigureLogging(builder => builder.UserNLog());
            host.ConfigureContainer(builder =>
            {
                builder.RegisterType<A>().As<I>();
                builder.RegisterType<B>().As<I>();
            });
            host.ConfigureWebHost(builder =>
            {
                builder.UseKestrel(options =>
                {
                    options.ListenAnyIP(10000);
                }).UseStartup<X>();
            });
            host.OnHostStarted += provider =>
            {
                foreach (var item in provider.GetServices<I>()) item.Do(5, 6);
            };
            await host.RunAsync();
        }
    }

    public class X
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

    // 定义接口
    public interface I
    {
        int Do(int a, int b);
    }

    // 定义实现
    public class A : I
    {
        public A() { }

        public int Do(int a, int b)
        {
            return a + b;
        }
    }

    // 定义实现
    public class B : I
    {
        public B() { }

        public int Do(int a, int b)
        {
            return a * b;
        }
    }
}

