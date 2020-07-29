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
    public class Root
    {
        public A A { get; set; }
        public B B { get; set; }
    }

    public class A
    {
        public int a1 { get; set; }
        public string a2 { get; set; }
    }

    public class B
    {
        public int b1 { get; set; }
        public string b2 { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                var host = new ServiceHost();
                host.AddJsonConfig($"{rootPath}a.json");
                host.AddJsonConfig($"{rootPath}b.json");
                host.InitService(provider =>
                {
                    TaskManager.CreateTask(() =>
                    {
                        var e = ServiceHost.Environment;
                        var r = e.Configuration.GetRoot<Root>();
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
