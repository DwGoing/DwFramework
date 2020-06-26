using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(DateTime.Parse("2020-06-27").GetTimeDiff());
                //var value = "osnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfjoiwejdweifnfosnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfjoiwejdweifnfosnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfjoiwejdweifnfosnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfjoiwejdweifnfosnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfjoiwejdweifnfosnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfjoiwejdweifnfosnfweiniocnownowenionweoinoiwneoidweonedowneoiweendwiondowne lwei we cweindenfwelinfiwlenfsfj1282689";
                //var keys = EncryptUtil.Rsa.GenerateKeyPair(RSAExtensions.RSAKeyType.Pkcs1, 1024);
                //var s1 = EncryptUtil.Rsa.EncryptWithPrivateKey(value, RSAExtensions.RSAKeyType.Pkcs1, keys.PrivateKey);
                //Console.WriteLine(s1);
                //var s2 = EncryptUtil.Rsa.Decrypt(s1, RSAExtensions.RSAKeyType.Pkcs1, keys.PrivateKey);
                //Console.WriteLine(s2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
