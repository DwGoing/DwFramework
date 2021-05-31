using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DwFramework.Core;
using Autofac;

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
                b.RegisterType<A>().As<I>();
            });
            host.ConfigureServices(b =>
            {
                b.AddTransient<B>();
            });
            host.OnHostStarted += p =>
            {
                p.GetService<B>().Do();
                p.GetService<B>().Do();
                p.GetService<B>().Do();
            };
            await host.RunAsync();
        }

        class C
        {
            public int X { get; set; }
        }

        interface I
        {
            void Do();
        }

        [Registerable(typeof(I))]
        class A : I
        {
            private ILogger _logger;
            private Guid ID;

            public A(ILogger<A> logger)
            {
                _logger = logger;
                ID = Guid.NewGuid();
            }

            public void Do()
            {
                _logger.LogDebug($"I'm A,{ID}");
            }
        }

        [Registerable(typeof(I), Lifetime.Singleton)]
        class B : I
        {
            private ILogger _logger;
            private Guid ID;

            public B(ILogger<B> logger)
            {
                _logger = logger;
                ID = Guid.NewGuid();
            }

            public void Do()
            {
                _logger.LogDebug($"I'm B,{ID}");
            }
        }
    }
}

