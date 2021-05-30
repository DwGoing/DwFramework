using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DwFramework.Core;
using Autofac;
using NLog.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.RegisterNLog();
        host.ConfigureContainer((_, b) =>
        {
            b.RegisterType<A>().As<I>();
        });
        host.ConfigureServices((_, b) =>
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
        private Guid ID;

        public A(IServiceProvider provider)
        {
            ID = Guid.NewGuid();
        }

        public void Do()
        {
            Console.WriteLine($"I'm A,{ID}");
        }
    }

    [Registerable(typeof(I), Lifetime.Singleton)]
    class B : I
    {
        public Guid ID;

        public B()
        {
            ID = Guid.NewGuid();
        }

        public void Do()
        {
            Console.WriteLine($"I'm B,{ID}");
        }
    }
}

