using System;
using DwFramework.Core;
using DwFramework.WebAPI;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(configFilePath: "Config.json");
                host.RegisterWebAPIService<Startup>();
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
