using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
            host.RegisterLog();
            host.RegisterType<CTest>();
            var provider = host.Build();
            provider.GetService<CTest>().M();
            Console.Read();
        }
    }

    public interface ITest
    {
        void M();
    }

    public class CTest : ITest
    {
        private readonly ILogger<CTest> _logger;

        public CTest(ILogger<CTest> logger, IEnvironment environment)
        {
            _logger = logger;
            var obj = environment.GetConfiguration().GetConfig("Test", new { A = "", B = 0 }.GetType());
            environment.GetConfiguration().SetConfig("Test:A", new { a = "a", b = 1 });
            Console.WriteLine(environment.GetConfiguration().GetSection("Test").Value);
        }

        public void M()
        {
            _logger.LogInformation("Helo");
        }
    }
}
