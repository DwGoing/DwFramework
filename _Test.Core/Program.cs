using System;
using System.Threading;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Helper;
using DwFramework.Core.Plugins;
using Microsoft.Extensions.Logging;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var keys = EncryptUtil.Rsa.GenerateKeyPair(RSAExtensions.RSAKeyType.Pkcs1, isPem: true);
                //Console.WriteLine(keys.PrivateKey);
                //Console.WriteLine(keys.PublicKey);
                //var a = EncryptUtil.Rsa.EncryptWithPublicKey("iosfn2930h92d3nnod32", RSAExtensions.RSAKeyType.Pkcs1, keys.PublicKey, true);
                //Console.WriteLine(a);
                //var b = EncryptUtil.Rsa.EncryptWithPrivateKey("iosfn2930h92d3nnod32", RSAExtensions.RSAKeyType.Pkcs1, keys.PrivateKey, true);
                //Console.WriteLine(b);
                //var c = EncryptUtil.Rsa.Decrypt(a, RSAExtensions.RSAKeyType.Pkcs1, keys.PrivateKey, true);
                //Console.WriteLine(c);
                //var d = EncryptUtil.Rsa.Decrypt(b, RSAExtensions.RSAKeyType.Pkcs1, keys.PrivateKey, true);
                //Console.WriteLine(d);
                var source = new CancellationTokenSource(5000);
                TaskManager.CreateTask(token =>
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested) return;
                        Console.WriteLine("z");
                        Thread.Sleep(1000);
                    }
                }, source.Token);
                Console.WriteLine("x");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
        }

        public void M()
        {
            _logger.LogInformation("Helo");
        }
    }
}
