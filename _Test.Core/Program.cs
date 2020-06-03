using System;

using DwFramework.Core;
using DwFramework.Core.Plugins;
using Microsoft.Extensions.Logging;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = EncryptUtil.Aes.EncryptToBase64("kjsdifnownefinei", "FkdcRHwHMsvj1Ijh", "eotLNWogMH2RtDfc");
            Console.WriteLine(a);
            var b = EncryptUtil.Aes.DecryptFromBase64(a, "FkdcRHwHMsvj1Ijh", "eotLNWogMH2RtDfc");
            Console.WriteLine(b);
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
        }

        public void M()
        {
            _logger.LogInformation("Helo");
        }
    }
}
