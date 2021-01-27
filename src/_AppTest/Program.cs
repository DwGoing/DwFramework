using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.ORM;
using System.Collections.Generic;

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
                host.RegisterORMService("ORM");
                host.OnInitialized += p =>
                {
                    var s = p.GetORMService();
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