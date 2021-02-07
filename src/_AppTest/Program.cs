using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

using System.IO;
using System.Threading.Tasks;

namespace _AppTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop);
                var s = new FileStream("Config.json", FileMode.Open, FileAccess.Read);
                host.AddJsonConfig(s);
                host.RegisterLog();
                host.OnInitialized += p =>
                {
                    var env = p.GetService<DwFramework.Core.Environment>();
                };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}