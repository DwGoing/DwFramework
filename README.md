# DwFramework

### 0x1 项目简介

基于Autofac的NetCore快速开发框架，这个框架旨在将服务注入简单化，把Autofac中常用的部分暴露出来，并融合了其他几个项目开发常用的组件。让整个开发的过程变得简单快速。当然，如果你有更复杂的业务需求，你可以直接引用Autofac来对本框架进行扩展。

在框架的设计方面，在DDD的基础上使用者可以为单个服务使用不同的框架设计，创建一个立体化的DDD模型，实现基础服务共享，高级服务隔离的效果。

---

### 0x2 组件列表

版本说明：NETVersion.ReleaseVersion.FixVersion

|            组件             |     说明      |                             版本                             |
| :-------------------------: | :-----------: | :----------------------------------------------------------: |
|      DwFramework.Core       |   核心组件    | [![](https://img.shields.io/badge/Nuget-5.0.1.12-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Core/) |
|    DwFramework.ORM     |    ORM组件    | [![](https://img.shields.io/badge/Nuget-5.0.1.18-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.ORM/) |
|    DwFramework.RabbitMQ     | RabbitMQ组件  | [![](https://img.shields.io/badge/Nuget-5.0.1.17-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RabbitMQ/) |
|       DwFramework.RPC       |    Rpc组件    | [![](https://img.shields.io/badge/Nuget-5.0.1.17-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RPC/) |
|     DwFramework.Socket      |  Socket组件   | [![](https://img.shields.io/badge/Nuget-5.0.1.17-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Socket/) |
|  DwFramework.TaskSchedule   | 任务调度组件  | [![](https://img.shields.io/badge/Nuget-5.0.1.12-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.TaskSchedule/) |
|     DwFramework.WebAPI      |  WebAPI组件   | [![](https://img.shields.io/badge/Nuget-5.0.1.17-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.WebAPI/) |
|    DwFramework.WebSocket    | WebSocket组件 | [![](https://img.shields.io/badge/Nuget-5.0.1.17-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.WebSocket/) |

|             插件             |     说明     |                             版本                             |
| :--------------------------: | :----------: | :----------------------------------------------------------: |
| DwFramework.ORM.Plugins |   ORM插件    | [![](https://img.shields.io/badge/Nuget-5.0.1.18-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.ORM.Plugins/) |
|   DwFramework.RPC.Plugins    |   Rpc插件    | [![](https://img.shields.io/badge/Nuget-5.0.1.17-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RPC.Plugins/) |
|  DwFramework.WebAPI.Plugins  |  WebAPI插件  | [![](https://img.shields.io/badge/Nuget-5.0.1.16-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.WebAPI.Plugins/) |

---

### 0x3 简单示例

```c#
public interface ITestInterface
{
    void TestMethod(string str);
}

[Registerable]
public class TestClass1 : ITestInterface
{
    public void TestMethod(string str)
    {
        Console.WriteLine($"TestClass1:{str}");
    }
}

[Registerable(typeof(ITestInterface))]
public class TestClass2 : ITestInterface
{
    public void TestMethod(string str)
    {
        Console.WriteLine($"TestClass2:{str}");
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.RegisterFromAssemblies();
        host.OnInitialized += p =>
        {
            p.GetService<TestClass1>().TestMethod("Hi");
            p.GetService<ITestInterface>().TestMethod("Hi");
        };
        await host.RunAsync();
    }
}
```