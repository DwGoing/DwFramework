using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using DwFramework.Core.Entities;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

// 定义接口
public interface ITestInterface
{
    void TestMethod(string str);
}

// 定义实现
[Registerable]
public class TestClass : ITestInterface
{
    // 要拦截的函数必须是virtual或override
    public virtual void TestMethod(string str)
    {
        Console.WriteLine($"TestClass:{str}");
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

class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.RegisterInterceptors(typeof(MyInterceptor));
        host.RegisterType<TestClass>().AddClassInterceptors(typeof(MyInterceptor));
        host.OnInitialized += p =>
        {
            p.GetService<TestClass>().TestMethod("Hi");
        };
        await host.RunAsync();
    }
}

