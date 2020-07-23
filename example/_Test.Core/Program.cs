using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using System.Text;
using System.Linq;
using System.Globalization;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.RegisterMemoryCache(3);
                host.RegisterFromAssemblies();
                host.Run();

                //var value = @"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";
                //var privateKey = @"MIIEpAIBAAKCAQEA0CJNhxLG2wOo57eF/Ornj7YXD6/eJoFq/Gc/KNm5EoVafv9Ug92jmUbPAPWIM9Ci07KaQd+J6T18VCO2WzFtYHV3uyu+cIUNSrZcBHXple5u3Z4/1le76x/DaMGCaGfRdtg6hoON33BV6+2Y09KcOqpFankMDUhOHGxej+w4s5Gz02/OPY1ubH8RN9+kSd7Cu/NBDnyV1losPC/aGXtV8IuVT4hF3E6AFywNNer4R59oUO+bKoDIwu89eTVoZR2haRgtryhq9VCDFVIOVXGup2w5CVHnOLJsvtMtqjKMO/wCbVYTmbtGaVQy2r7W2FT+tbCUdxH3sc0kLYXtVfwhtwIDAQABAoIBAQCsqCgkKwFnYgvV3Tp7auqZHvbWfpAM5UM5CvUsECElKha+T1Vu5of2ePTz2LsaMLNCZmDs0GF5aRYgPlfiIoiXghrG3Czo7pbuKYT/9kjFpbu2gLZ4OuOa0wipeA2USrtKmWlDeRJSDsBYLQugfJA5YlKfVrcWtaqGjaeMQOtwmabvcswIYDe1saiBCRXr7/Iv4KFgpVMfRYls1wzwaDv393RaUKmSjxmIRLW+k75jKdglkcGAK3RqLLs/CIYQ/mHWxWQsS2xf1SN+vK3CNNXsVNO000i7uVwS2e9Ou/P9wYhOsADs4Axf+sTPUnDCZvzGblmpqtUzD6ErAcC8VKtxAoGBAP8O1c1lNI9yp65SVMgCFx8HaOn9U+wruKWJ6O0VeiEaJLasgwF05E+nEImM0Q/Scq5EoCg6mh2aY95iOam2aBwVgah9qHnrEJ+wChclxGviNNfDm1zcAeltUO2yHD4dtFPEIl+yXD3KUwlQRgFyvo+XhA/HOwsDTySDFSnAKrk1AoGBANDnGYVO6gjFmshlljRIlLohJnqVmvmtE5IUn5NCMX+TNakzvwpRzrtLc+1ZSf1yBMosgn88poDdjkGHZL8ZXwtEsV/+v/zkFQW0aEtFs67cVUfktOKSjHG+evqMZlekEcQTXZz1UG4wulZ7RBqQxM0PybBk5Dys9m43claE3ni7AoGBANq6VCuiIOLrhlT+Eeq7sCxR5GzVbITaMaz0iaXXhzaf/uARLP+wyKJuOMZc1mRlKye7fkVBjCza2844Gg8qeDmtT9W4fSSgq07mXqDfKIUEJiDqhG+r1I/jyUUuOv4h5yT2zCuY/3WV7oPMLVzMlBL78qq9Rir5mYNMTnfRblIJAoGADUQ/6KlkT35NICDjcxqQ52knim1p1CVbstFAeRehERsGM2Kn5T3gxSA7kn0zJ7dP+o7tEquFX3WyjRLOIRy5XnvUT+ZbxvGtLBmS7gTVLmurts8dda4c4TRZlwPHlBVFU5BvR4KEwxqxGsDlSFKdTPCNvHgLzpalZ8Z5qmjxv/UCgYAC3J1BB4h14e3mFfQ8wAI2KJudL2zHbWX3EG6DG+PLAfBNmw4PyYRycfViXBGZvYTka355vmxDFjkSQea7yIDk0oiL/QNQPXQF9BRcHMBgopW4eaj81RqVWv8P8QCMH4u1cE6mD5KwT2DB1cXMslkFS6oCN85nokFJZ6Jt/O0Ucg==";
                //var v = EncryptUtil.Rsa.Decrypt(value, RSAExtensions.RSAKeyType.Pkcs1, privateKey);
                //Console.WriteLine(v);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }

    [Registerable(typeof(A), Lifetime.Singleton, isAutoActivate: true)]
    public class A
    {
        ICache _cache;

        public A(ICache cache)
        {
            _cache = cache;
            _cache.Set("1", 123, TimeSpan.FromSeconds(10));
            TaskManager.CreateTask(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(cache.Get<int>("1"));
                }
            });
        }
    }
}
