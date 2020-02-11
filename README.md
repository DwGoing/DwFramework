# DwFramework
### 0x1 项目简介

基于Autofac的Dotnet Core快速开发框架，这个框架旨在将服务注入简单化，把Autofac中常用的部分暴露出来，并融合了其他几个项目开发常用的组件。让整个开发的过程变得简单快速，（不能说学习是浪费时间，只是说有时候需要快速完成开发🤦‍♂️）。当然，如果你有更复杂的业务需求，你可以直接引用Autofac来对本框架进行扩展。

在框架的设计方面，在DDD的基础上使用者可以为单个服务使用不同的框架设计，创建一个立体化的DDD模型。下层框架（单个服务中的框架）中可以通过IServiceProvider来获取上层框架的服务，而反过来是不行的。这样的设计是为了实现基础服务共享，高级服务隔离的效果。

---

### 0x2 组件列表

|         组件          |     说明      | 示例 |
| :-------------------: | :-----------: | :--: |
|   DwFramework.Core    |   核心组件    |      |
|   DwFramework.Http    |  WebAPI组件   |      |
| DwFramework.WebSocket | WebSocket组件 |      |
|                       |               |      |
|                       |               |      |
|                       |               |      |

---

### 0x3 简单示例

```c#
// Test.cs
using System;
using Microsoft.Extensions.Configuration;
using DwFramework.Core.Models;

namespace Test
{
  	public interface ITestInterface
    {
        void TestMethod(string str);
    }
  
    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass1 : ITestInterface
    {
        public TestClass1()
        {
            Console.WriteLine("TestClass1已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass1:{str}");
        }
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass2 : ITestInterface
    {
        public TestClass2()
        {
            Console.WriteLine("TestClass2已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass2:{str}");
        }
    }
}
```

```c#

// Program.cs
using DwFramework.Core;
using DwFramework.Core.Extensions;

class Program
{
    static void Main(string[] args)
    {
        ServiceHost host = new ServiceHost();
        host.RegisterFromAssembly("Test"); // 从程序集注入
        var provider = host.Build();
        var service = provider.GetService<ITestInterface, TestClass1>();
        service.TestMethod("helo");
    }
}
```