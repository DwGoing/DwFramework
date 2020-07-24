using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using System.Text;
using System.Linq;
using System.Globalization;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterMemoryCache(3);
                host.RegisterFromAssemblies();
                host.InitService(provider =>
                {
                    TaskManager.CreateTask(() =>
                    {
                        using (var scope = ServiceHost.CreateLifetimeScope())
                        {
                            scope.GetService<A>().AddData();
                        }
                        using (var scope = ServiceHost.CreateLifetimeScope())
                        {
                            scope.GetService<A>().GetData();
                        }
                    });
                });
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }

    [Registerable(typeof(A))]
    public class A
    {
        readonly ICache _cache;

        public A(ICache cache)
        {
            _cache = cache;
        }

        public void AddData()
        {
            var timer = new DwFramework.Core.Plugins.Timer();
            for (int i = 0; i < 1000000; i++)
            {
                _cache.Set(i.ToString(), i);
            }
            Console.WriteLine(timer.GetTotalMilliseconds() + "ms");
        }

        public void GetData()
        {
            Console.WriteLine(_cache.Get<int>("34986"));
        }
    }
}
