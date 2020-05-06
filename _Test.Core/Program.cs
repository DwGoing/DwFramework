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
            ServiceHost host = new ServiceHost();
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

        public CTest(ILogger<CTest> logger)
        {
            _logger = logger;
        }

        public void M()
        {
            _logger.LogInformation("Helo");
        }
    }
}
