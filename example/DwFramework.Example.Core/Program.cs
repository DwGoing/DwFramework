using System;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.Example.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var tag = 8;
            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var example = assembly.GetType($"{assembly.FullName.Split(',')[0]}.Example{tag}");
                    if (example == null) continue;
                    example.GetMethod("Invoke").Invoke(null, null);
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



    }

    /// <summary>
    /// 快速开始
    /// </summary>
    public class Example1
    {
        public static void Invoke()
        {
            var host = new ServiceHost(); // 初始化服务主机
            host.Register(context => new S1()); // 注册服务
            host.RegisterType<S2>(); // 注册服务
            host.RegisterFromAssemblies(); // 注册服务
            host.OnInitialized += provider => provider.GetService<S1>().Do();
            host.OnInitialized += provider => provider.GetService<S2>().Do();
            host.OnInitialized += provider => provider.GetService<S3>().Do();
            host.Run();
        }

        class S1
        {
            public void Do() => Console.WriteLine("s1");
        }

        class S2
        {
            public void Do() => Console.WriteLine("s2");
        }

        [Registerable]
        class S3
        {
            public void Do() => Console.WriteLine("s3");
        }
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    public class Example2
    {
        public static void Invoke()
        {
            var host = new ServiceHost(); // 初始化服务主机
            host.RegisterType<S1, IS>(); // 注册服务
            host.RegisterType<S2, IS>(); // 注册服务
            host.OnInitialized += provider => provider.GetService<IS>().Do(); // 默认获取到的是最后注册的IS实现
            host.Run();
        }

        interface IS
        {
            void Do();
        }

        class S1 : IS
        {
            public void Do() => Console.WriteLine("s1");
        }

        class S2 : IS
        {
            public void Do() => Console.WriteLine("s2");
        }
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    public class Example3
    {
        public static void Invoke()
        {
            var host = new ServiceHost(); // 初始化服务主机
            host.Register(context => new S("hello")); // 注册服务
            host.OnInitialized += provider => provider.GetService<S>().Do();
            host.Run();
        }

        class S
        {
            readonly string _tag;

            public S(string tag)
            {
                _tag = tag;
            }

            public void Do() => Console.WriteLine($"s_{_tag}");
        }
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    public class Example4
    {
        public static void Invoke()
        {
            var host = new ServiceHost(); // 初始化服务主机
            host.RegisterFromAssemblies(); // 注册服务
            host.OnInitialized += provider => provider.GetService<IS>().Do(); // 默认获取到的是最后注册的IS实现
            host.Run();
        }

        interface IS
        {
            void Do();
        }

        [Registerable(typeof(IS))]
        class S1 : IS
        {
            public void Do() => Console.WriteLine("s1");
        }

        [Registerable(typeof(IS))]
        class S2 : IS
        {
            public void Do() => Console.WriteLine("s2");
        }
    }

    /// <summary>
    /// AOP插件
    /// </summary>
    public class Example5
    {
        public static void Invoke()
        {
            var host = new ServiceHost(); // 初始化服务主机
            host.RegisterInterceptors(typeof(MyInterceptor)); // 注册拦截器
            host.RegisterType<S>().AddClassInterceptors(typeof(MyInterceptor)); // 注册服务并添加拦截器
            host.OnInitialized += provider => provider.GetService<S>().Do();
            host.Run();
        }

        public class S
        {
            // 要拦截的函数必须是虚函数或者重写函数
            public virtual void Do()
            {
                Console.WriteLine("s");
            }
        }

        /// <summary>
        /// 构造拦截器
        /// 1.继承BaseInterceptor
        /// 2.重写OnCalling(CallInfo info)函数
        /// 3.重写OnCalled(CallInfo info)函数
        /// </summary>
        public class MyInterceptor : BaseInterceptor
        {
            public override void OnCalling(CallInfo info)
            {
                Console.WriteLine("OnCalling");
            }

            public override void OnCalled(CallInfo info)
            {
                Console.WriteLine("OnCalled");
            }
        }
    }

    /// <summary>
    /// Log插件
    /// </summary>
    public class Example6
    {
        public static void Invoke()
        {
            var host = new ServiceHost(); // 初始化服务主机
            host.RegisterLog(); // 注册服务
            host.OnInitialized += provider =>
            {
                var logger = provider.GetLogger<Example6>();
                logger.LogInformation("Example6");
            };
            host.Run();
        }
    }

    /// <summary>
    /// MemoryCache插件
    /// </summary>
    public class Example7
    {
        public static void Invoke()
        {
            var host = new ServiceHost(); // 初始化服务主机
            host.RegisterMemoryCache(); // 注册服务
            host.OnInitialized += provider =>
            {
                var cache = provider.GetCache();
                cache.Set("test", new { A = "1", B = 2 }); // 插入数据
                var value = cache.Get("test"); // 获取数据
            };
            host.Run();
        }
    }

    /// <summary>
    /// Encryption插件
    /// </summary>
    public class Example8
    {
        public static void Invoke()
        {
            var str = "DwFramework";
            var md5 = MD5.Encode(str); // MD5
            var aes = AES.EncryptToHex(str, "1234567890abcdef", "1234567890abcdef"); // AES
            var raw = AES.DecryptFromHex(aes, "1234567890abcdef", "1234567890abcdef");
            var keys = RSA.GenerateKeyPair(RSAExtensions.RSAKeyType.Pkcs8, isPem: true); // Pem格式密钥
            var rsa = RSA.EncryptWithPublicKey(str, RSAExtensions.RSAKeyType.Pkcs8, keys.PublicKey, true); // RSA
            raw = RSA.Decrypt(rsa, RSAExtensions.RSAKeyType.Pkcs8, keys.PrivateKey, true);
        }
    }
}



