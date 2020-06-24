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
                var value = "osnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfjoiwejdweifnf ihihiwienrinifnwieniwenfinweieniwenfiwenfiwenfiwenfin mv ekllirnwliennrin i";
                var keys = EncryptUtil.Rsa.GenerateKeyPair(RSAExtensions.RSAKeyType.Pkcs1, 1024);
                var s1 = EncryptUtil.Rsa.EncryptWithPrivateKey(value, RSAExtensions.RSAKeyType.Pkcs1, keys.PrivateKey);
                Console.WriteLine(s1);
                var s2 = EncryptUtil.Rsa.Decrypt(s1, RSAExtensions.RSAKeyType.Pkcs1, keys.PrivateKey);
                Console.WriteLine(s2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
