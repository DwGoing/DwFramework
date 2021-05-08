using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using SqlSugar;

namespace _AppTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("Config.json");
                host.RegisterLog();
                host.RegisterFromAssemblies();
                host.OnInitializing += p =>
                {
                };
                host.OnInitialized += p =>
                {
                };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }
        }
    }
}

