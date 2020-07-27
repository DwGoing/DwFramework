using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.WebAPI.Extensions;

namespace _Test.WebAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceHost host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                host.RegisterLog();
                host.RegisterWebAPIService();
                host.InitService(provider => provider.InitWebAPIServiceAsync<Startup>());
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
