using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace CoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureLogging(builder => builder.UserNLog());
            host.ConfigureContainer(builder =>
            {
                builder.RegisterType<A>().As<I>();
                builder.RegisterType<B>().As<I>();
            });
            host.OnHostStarted += provider =>
            {
                foreach (var item in provider.GetServices<I>()) Console.WriteLine(item.Do(5, 6));
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

