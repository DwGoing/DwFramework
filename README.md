# DwFramework

### 0x1 项目简介

基于Autofac的NetCore快速开发框架，这个框架旨在将服务注入简单化，把Autofac中常用的部分暴露出来，并融合了其他几个项目开发常用的组件。让整个开发的过程变得简单快速。当然，如果你有更复杂的业务需求，你可以直接引用Autofac来对本框架进行扩展。

---

### 0x2 组件列表

版本说明：NETVersion.ReleaseVersion.FixVersion

|            组件             |     说明      |                             版本                             |
| :-------------------------: | :-----------: | :----------------------------------------------------------: |
|      DwFramework.Core       |   核心库    | [![](https://img.shields.io/badge/Nuget-5.1.0.26-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Core/) |
|    DwFramework.SqlSugar     |    SqlSugar封装库    | [![](https://img.shields.io/badge/Nuget-5.1.0.26-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.SqlSugar/) |
|    DwFramework.RabbitMQ     | RabbitMQ封装库  | [![](https://img.shields.io/badge/Nuget-5.1.0.26-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RabbitMQ/) |
|       DwFramework.Quartz       |    Quartz封装库    | [![](https://img.shields.io/badge/Nuget-5.1.0.26-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Quartz/) |
|     DwFramework.Web      |  网络库   | [![](https://img.shields.io/badge/Nuget-5.1.0.26-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Web/) |

---

### 0x3 简单示例

```c#
class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.ConfigureLogging(builder => builder.UserNLog());
        host.RegisterFromAssemblies();
        host.OnHostStarted += provider =>
        {
            foreach (var item in provider.GetServices<I>())
                Console.WriteLine(item.Do(5, 6));
        };
        await host.RunAsync();
    }
}

// 定义接口
public interface I
{
    int Do(int a, int b);
}

// 定义实现
[Registerable(typeof(I))]
public class A : I
{
    public A() { }

    public int Do(int a, int b)
    {
        return a + b;
    }
}

// 定义实现
[Registerable(typeof(I))]
public class B : I
{
    public B() { }

    public int Do(int a, int b)
    {
        return a * b;
    }
}
```