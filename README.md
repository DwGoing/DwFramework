# DwFramework

### 0x1 项目简介

基于Autofac的Dotnet Core快速开发框架，这个框架旨在将服务注入简单化，把Autofac中常用的部分暴露出来，并融合了其他几个项目开发常用的组件。让整个开发的过程变得简单快速。当然，如果你有更复杂的业务需求，你可以直接引用Autofac来对本框架进行扩展。

在框架的设计方面，在DDD的基础上使用者可以为单个服务使用不同的框架设计，创建一个立体化的DDD模型。下层框架（单个服务中的框架）中可以通过IServiceProvider来获取上层框架的服务，而反过来是不行的。这样的设计是为了实现基础服务共享，高级服务隔离的效果。

---

### 0x2 组件列表

版本说明：NETVersion.ReleaseVersion.FixVersion

|           组件           |     说明      |                             版本                             |
| :----------------------: | :-----------: | :----------------------------------------------------------: |
|     DwFramework.Core     |   核心组件    | [![](https://img.shields.io/badge/Nuget-2.2.55.9213-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Core/) |
|     DwFramework.ORM      |    ORM组件    | [![](https://img.shields.io/badge/Nuget-2.2.55.9215-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.ORM/) |
|   DwFramework.RabbitMQ   | RabbitMQ组件  | [![](https://img.shields.io/badge/Nuget-2.2.55.9216-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RabbitMQ/) |
|     DwFramework.RPC      |    RPC组件    | [![](https://img.shields.io/badge/Nuget-2.2.55.9217-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Rpc/) |
|    DwFramework.Socket    |  Socket组件   | [![](https://img.shields.io/badge/Nuget-2.2.55.9217-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Socket/) |
| DwFramework.TaskSchedule | 任务调度组件  | [![](https://img.shields.io/badge/Nuget-2.2.55.9218-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.TaskSchedule/) |
|    DwFramework.WebAPI    |  WebAPI组件   | [![](https://img.shields.io/badge/Nuget-2.2.55.9219-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.WebAPI/) |
|  DwFramework.WebSocket   | WebSocket组件 | [![](https://img.shields.io/badge/Nuget-2.2.55.9220-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.WebSocket/) |

|            插件            |    说明    |                             状态                             |
| :------------------------: | :--------: | :----------------------------------------------------------: |
|  DwFramework.ORM.Plugins   |  ORM插件   | [![](https://img.shields.io/badge/Nuget-2.2.55.9220-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Plugins.Database/) |
|  DwFramework.RPC.Plugins   |  RPC插件   | [![](https://img.shields.io/badge/Nuget-2.2.55.9221-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Plugins.Rpc/) |
| DwFramework.WebAPI.Plugins | WebAPI插件 | [![](https://img.shields.io/badge/Nuget-2.2.55.9219-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Plugins.WebAPI/) |

---

### 0x3 简单示例

```c#
using System;
using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace _AppTest
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

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterFromAssemblies();
                host.OnInitialized += p =>
                {
                    var ts = p.GetServices<ITestInterface>();
                    foreach (var item in ts)
                    {
                        item.TestMethod("Hello!");
                    }
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
```
