using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

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
                    int i = 0;
                    int j = 0;
                    TaskManager.CreateTask(() =>
                    {
                        using (var scope = ServiceHost.CreateLifetimeScope())
                        {
                            var cache = scope.GetService<ICache>();
                            var timer = new DwFramework.Core.Plugins.Timer();
                            for (; i < 1000; i++)
                            {
                                for (; j < 1000; j++)
                                {
                                    cache.HSet($"k{i}", $"f{j}", i + j);
                                }
                            }
                            Console.WriteLine($"{timer.GetTotalMilliseconds()}ms");
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
}
