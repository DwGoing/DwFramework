using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using NLog;
using DwFramework.Core;
using DwFramework.Core.AOP;

namespace CoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.AddJsonConfiguration("Config.json", reloadOnChange: true);
            host.ConfigureLogging(builder => builder.UserNLog());
            host.ConfigureContainer(builder =>
            {
                builder.Register(c => new LoggerInterceptor(invocation => (
                    $"{invocation.TargetType.Name}InvokeLog",
                    LogLevel.Debug,
                    "\n========================================\n"
                    + $"Method:\t{invocation.Method}\n"
                    + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
                    + $"Return:\t{invocation.ReturnValue}\n"
                    + "========================================"
                )));
                builder.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
                builder.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
            });
            host.OnHostStarted += provider =>
            {
                ServiceHost.ParseConfiguration<string>("ConnectionString");
                foreach (var item in provider.GetServices<I>()) item.Do(5, 6);
            };
            await host.RunAsync();
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

