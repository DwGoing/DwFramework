using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.SqlSugar;

namespace SqlSugarExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            host.AddJsonConfig("Config.json");
            host.ConfigureSqlSugar();
            host.ConfigureLogging(builder => builder.UserNLog());
            host.OnHostStarted += p =>
            {
                var s = p.GetSqlSugarService();
                var c = s.CreateConnection("Main");
            };
            await host.RunAsync();
        }
    }
}
