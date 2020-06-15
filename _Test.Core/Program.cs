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
                var token = new CancellationTokenSource();
                var task = Task.Run(() =>
                {
                    Thread.Sleep(10000);
                    token.Token.ThrowIfCancellationRequested();
                    Console.WriteLine(123);
                }, token.Token);
                Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    token.Cancel();
                });
                task.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
