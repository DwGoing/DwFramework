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
            //var a = EncryptUtil.Rsa.EncryptWithPrivateKeyToBase64("iosfn2930h92d3nnod32", @"MIICXAIBAAKBgQCR7ftc8H8U07NX6/yKtJcwNzL87Fu2P6lJjOUB1zCOnsBZpwSBOcl53brSFGYrk7U8Mi0Kk5H2E06h2uJ19Rz/QtYYv6ciE05MSMjsA6UY90wurdLGAxxJRYfDawP1FA2NwVJhK531tszbVqXPGYrknA6DyJp7O/6w7eSwthWvCwIDAQABAoGAA+GUTh2YBlWuJIUPNrpkqc509nkOKJX0qpMmtDqYPPvjtdYsxf845Gfk24ymecxtiZXjHYUhT6Vyz5Hy2tOvYaybH72bZxT+hDHDXIH27FXCB70Lt5/aYJURckLE5A0vM1RI199nfWoeaedvo8R0GLCdgu/a57+r4qrOMYldtPECQQDjCHkQwUM43wVgstU4d/8BxWX6k7+Drw5C2biFILuacMOZ6tJCznshEYYRV3m1xal0Jt6t1/Pc9lgRtowrZaU7AkEApIxyFIzkpTMEifLod4iqVeDrO6Gzwp6grCE70XmNmeWP272t9ev0SkuEbatZ2IXk6W+PmQrqbw+aCuGJ7CtAcQJAYsgV/P9J413ONjO5aDd1weyEoZFVm9M4Dkgy6+HBHsJ/qOGuGJlEo6+/OJ5p+3gEceBTtSooFfUtfo+Bz4QLbwJAJcRegrz71bbp+ceY96aUgfWHyD4LBkZmVluiYSfnCkWYSTU18lXf0hWXJZzImFvS+Ik0iknpGOiZ0JmHkH804QJBAISYiXtxK2tNCe+qAoNmHVOkcUBrwsN1TwVR7Bi6cF1zCFoCbdmO426dHDF/QYsJ7OJwqNYXqkGgn0LAy3MmCms=");
            //var a = EncryptUtil.Rsa.EncryptWithPublicKeyToBase64("iosfn2930h92d3nnod32", @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCR7ftc8H8U07NX6/yKtJcwNzL87Fu2P6lJjOUB1zCOnsBZpwSBOcl53brSFGYrk7U8Mi0Kk5H2E06h2uJ19Rz/QtYYv6ciE05MSMjsA6UY90wurdLGAxxJRYfDawP1FA2NwVJhK531tszbVqXPGYrknA6DyJp7O/6w7eSwthWvCwIDAQAB");
            //Console.WriteLine(a);
            //var b = EncryptUtil.Aes.DecryptFromHex(a, "FkdcRHwHMsvj1Ijh", "eotLNWogMH2RtDfc");
            //Console.WriteLine(b);
            Console.ReadLine();
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
