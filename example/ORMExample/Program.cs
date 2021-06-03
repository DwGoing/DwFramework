using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.ORM;

namespace ORMExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.ConfigureORMWithJson("Config.json");
            host.ConfigureLogging(builder => builder.UserNLog());
            host.OnHostStarted += p =>
            {
                var s = p.GetORMService();
                var c = s.CreateConnection("Main");
            };
            await host.RunAsync();
        }
    }
}
