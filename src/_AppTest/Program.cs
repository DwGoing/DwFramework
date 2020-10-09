using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.WebAPI;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterLog();
                host.RegisterWebAPIService<Startup>("webapi.json");
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
