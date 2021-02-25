using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.Socket;

using System.IO;
using System.Threading.Tasks;

namespace _AppTest
{
    public class A
    {
        public int a { get; set; }
        public string b { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("Config.json");
                // host.RegisterLog();
                host.RegisterTcpService("Tcp");

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