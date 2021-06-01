using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using DwFramework.Core;
using DwFramework.Plugins.AOP;

namespace CoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureLogging((_, builder) => builder.UserNLog());
            host.ConfigureContainer(b =>
            {
                b.Register(c => new LogInterceptor(LogLevel.Debug));
                b.RegisterType<TestClass>().InterceptedBy(typeof(LogInterceptor)).EnableClassInterceptors();
            });
            host.OnHostStarted += p =>
            {
                p.GetService<TestClass>().TestMethod("Hello");
            };
            await host.RunAsync();
        }
    }

    // 定义接口
    public interface ITestInterface
    {
        void TestMethod(string str);
    }

    // 定义实现
    public class TestClass : ITestInterface
    {
        // 要拦截的函数必须是virtual或override
        public virtual void TestMethod(string str)
        {
            Console.WriteLine($"TestClass:{str}");
        }
    }
}

