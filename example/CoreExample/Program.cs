using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using NLog;
using DwFramework.Core;
using DwFramework.Plugins.AOP;

namespace CoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            // 配置Logger
            host.ConfigureLogging((_, builder) => builder.UserNLog());
            host.ConfigureContainer(b =>
            {
                // 使用日志拦截器
                b.Register(c => new LoggerInterceptor(invocation => (
                    $"{invocation.TargetType.Name}InvokeLog",
                    LogLevel.Debug,
                    "\n========================================\n"
                    + $"Method:\t{invocation.Method}\n"
                    + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
                    + $"Return:\t{invocation.ReturnValue}\n"
                    + "========================================"
                )));
                b.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
                b.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
            });
            host.OnHostStarted += p =>
            {
                foreach (var item in p.GetServices<I>()) item.Do(5, 6);
            };
            await host.RunAsync();
        }
    }

    // 定义接口
    [Intercept(typeof(LoggerInterceptor))]
    public interface I
    {
        int Do(int a, int b);
    }

    // 定义实现
    public class A : I
    {
        public int Do(int a, int b)
        {
            return a + b;
        }
    }

    // 定义实现
    public class B : I
    {
        public int Do(int a, int b)
        {
            return a * b;
        }
    }
}

