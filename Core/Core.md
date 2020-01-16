# DwFramework.Core

### DwFramework核心库

0x1 当开始一个项目时，先要初始化服务主机ServiceHost。

```c#
ServiceHost host = new ServiceHost();
```

0x2 一般我们在服务初始化阶段，会读取配置文件。

```c#
// 注册全局配置文件
host.RegisterConfiguration("{文件路径}", "{文件名}");
```

0x3 如果需要记录日志的话，我们提供了NLog组件来满足，只需在注册了NLog服务后在运行根目录中创建nlog.config文件即可。

```c#
// 注册NLog组件
host.RegisterNLog();
```

0x4 ServiceHost提供了多种方式的服务注入，也是对Autofac的服务注入作了一定程度的封装，尽可能地使注入方式更容易让人理解其内部实现。

```c#
// 从程序集注入
host.RegisterFromAssembly("{程序集名}");
```

当使用RegisterFromAssembly来注入使，我们需要先使用[Registerable]对程序集中需要注入的类进行标识。

```c#
[Registerable(typeof(ITestInterface), Lifetime.Singleton)]
public class TestClass1 : ITestInterface
{
    private readonly ILogger _logger;

    public TestClass1(IConfiguration configuration, ILogger logger)
    {
        _logger = logger;
        _logger.Debug("TestClass1已注入");
    }

    public void TestMethod(string str)
    {
        _logger.Debug($"TestClass1:{str}");
    }
}
```