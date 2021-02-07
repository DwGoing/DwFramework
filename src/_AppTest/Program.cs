using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.WebAPI;

using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Autofac;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterWebAPIService<Startup>("WebAPI");
                host.OnInitialized += p =>
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        p.StopWebAPIService();
                        Console.WriteLine(1);
                        await Task.Delay(10000);
                        _ = p.RunWebAPIServiceAsync<Startup>("WebAPI1");
                        Console.WriteLine(2);
                    });
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}