using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.WebAPI;

namespace _AppTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.RegisterWebAPIService<Startup>(new WebAPIService.Config()
                {
                    Listen = new Dictionary<string, string>() {
                        { "http" , ":5000"}
                    }
                });
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