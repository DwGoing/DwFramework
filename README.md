# DwFramework
### 0x1 项目简介

基于Autofac的Dotnet Core快速开发框架

---

### 0x2 组件列表

|         组件          |     说明      | 示例 |
| :-------------------: | :-----------: | :--: |
|   DwFramework.Core    |   核心组件    |      |
|   DwFramework.Http    |  WebAPI组件   |      |
| DwFramework.WebSocket | WebSocket组件 |      |
| DwFramework.Database  | Database组件  |      |
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