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
                var task = TaskManager.CreateTask(token =>
                {
                    try
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            Thread.Sleep(3000);
                            Console.WriteLine("z");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }, out var token);
                Thread.Sleep(5000);
                token.Cancel();
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
