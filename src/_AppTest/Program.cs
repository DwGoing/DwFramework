using System;
using System.Text;
using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using DwFramework.RPC;
using DwFramework.RPC.Plugins;
using DwFramework.Socket;
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
                host.RegisterWebAPIService<Startup>("WebAPI.json");
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
