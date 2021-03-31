using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

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