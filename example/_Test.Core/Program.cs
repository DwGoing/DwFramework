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
                            var cache = scope.GetService<ICache>();
                            cache.HSet("test", "1", 1);
                            cache.HSet("test", "2", "2");
                        }
                        using (var scope = ServiceHost.CreateLifetimeScope())
                        {
                            var cache = scope.GetService<ICache>();
                            Console.WriteLine(cache.HGetAll("test")["1"].ToJson());
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
