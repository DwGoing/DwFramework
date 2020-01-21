# DwFramework.WebSocket

## DwFramework WebSocket库

### 0x1 初始化

当开始一个项目时，先要初始化服务主机ServiceHost。

```c#
ServiceHost host = new ServiceHost();
```

### 0x2 内置服务

1. 一般我们在服务初始化阶段，会读取配置文件。

```c#
// 注册全局配置文件
host.RegisterConfiguration({文件路径}, {文件名});
```

2. 如果需要记录日志的话，我们提供了NLog组件来满足，只需在注册了NLog服务后在运行根目录中创建nlog.config文件即可。

```c#
// 注册NLog组件
host.RegisterNLog();
```

### 0x3 服务注入

ServiceHost提供了多种方式的服务注入，也是对Autofac的服务注入作了一定程度的封装，尽可能地使注入方式更容易让人理解其内部实现。为了方便后面的案例说明，我们先定义示例中使用到的接口和类型。类型TestClass1和TestCless2均实现了接口ITestInterface。

```c#
namespace Test
{
    public interface ITestInterface
    {
        void TestMethod(string str);
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass1 : ITestInterface
    {
        private readonly ILogger _logger;

        public TestClass1(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("TestClass1已注入");
        }

        public void TestMethod(string str)
        {
            _logger.Debug($"TestClass1:{str}");
        }
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass2 : ITestInterface
    {
        private readonly ILogger _logger;

        public TestClass2(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("TestClass2已注入");
        }

        public void TestMethod(string str)
        {
            _logger.Debug($"TestClass2:{str}");
        }
    }
}
```

1. 批量注入。当我们需要现实批量注入时，我们将通过RegisterFromAssembly来注入服务。

```c#
host.RegisterFromAssembly({程序集名});
```

当使用RegisterFromAssembly来注入使，我们需要先使用Registerable对程序集中需要注入的类进行标识。

```c#
[Registerable({接口类型}, {生命周期},{是否初始化})]
```

```c#
[Registerable(typeof(ITestInterface), Lifetime.Singleton)] // 标记该类型实现的接口及实现类型
public class TestClass1 : ITestInterface
{
    public TestClass1(ILogger logger)
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

2. 单个类型注入。单个服务的注入我们可以使用RegisterType来注入。

```c#
host.RegisterType<TestClass1>().As<ITestInterface>(); // Autofac原生模式
host.RegisterType<ITestInterface, TestClass2>();
```