#define Example5

using System;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.Example.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
#if Example1
                Example1.Invoke();
#elif Example2
                Example2.Invoke();
#elif Example3
                Example3.Invoke();
#elif Example4
                Example4.Invoke();
#elif Example5
                Example5.Invoke();
#endif
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
}



