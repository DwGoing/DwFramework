# DwFramework.Core

```shell
PM> Install-Package DwFramework.Core
或
> dotnet add package DwFramework.Core
```

## DwFramework 核心库

### 0x1 初始化

当开始一个项目时，先要初始化服务主机ServiceHost。

```c#
ServiceHost host = new ServiceHost(EnvironmentType.Develop, $"配置文件路径");
```

### 0x2 内置服务

1. 如果需要记录日志的话，我们提供了NLog组件来满足，只需在注册了NLog服务后在运行根目录中创建nlog.config文件即可。

```c#
// 注册Log组件
host.RegisterLog();
```

NLog.config示例

```xml
<?xml version="1.0" encoding="utf-8"?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <extensions>
        <add assembly="NLog.MailKit" />
    </extensions>
    <targets>
        <!--使用可自定义的着色将日志消息写入控制台-->
        <target name="ColorConsole" xsi:type="ColoredConsole" layout="[${level}] ${date:format=yyyy\-MM\-dd HH\:mm\:ss}:${message} ${exception:format=message}" />
        <target name="Mail" xsi:type="Mail" smtpServer="smtp.mxhichina.com" smtpPort="465" smtpAuthentication="Basic" smtpUserName="账号" smtpPassword="密码" enableSsl="true" addNewLines="true" from="斑码网络&lt;bancode@bancode.net&gt;"
            to="260049383@qq.com" subject="邮件主题" header="===============" body="${newline}${message}${newline}" footer="================" />
        <!--此部分中的所有目标将自动异步-->
        <target name="AsyncFile" xsi:type="AsyncWrapper">
            <!--项目日志保存文件路径说明fileName="${basedir}/保存目录，以年月日的格式创建/${shortdate}/${记录器名称}-${单级记录}-${shortdate}.txt"-->
            <target name="log_file" xsi:type="File" fileName="${basedir}/Logs/${shortdate}/${logger}/${level}.txt" layout="[${level}] ${longdate} | ${message} ${onexception:${exception:format=message} ${newline} ${stacktrace} ${newline}" archiveFileName="${basedir}/archives/${logger}-${level}-${shortdate}-{#####}.txt" archiveAboveSize="102400" archiveNumbering="Sequence" concurrentWrites="true" keepFileOpen="false" />
        </target>
    </targets>
    <!--规则配置,final - 最终规则匹配后不处理任何规则-->
    <rules>
        <logger name="*" minlevel="Debug" writeTo="ColorConsole" />
        <logger name="*" minlevel="Info" writeTo="Mail" />
        <logger name="*" minlevel="Info" writeTo="AsyncFile" />
        <logger name="Microsoft.*" minlevel="Info" writeTo="" final="true" />
    </rules>
</nlog>
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
        private readonly ILogger<TestClass1> _logger;

        public TestClass1(ILogger<TestClass1> logger)
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
        private readonly ILogger<TestClass2> _logger;

        public TestClass2(ILogger<TestClass2> logger)
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

### 0x4 使用拦截器

```c#
public interface ITest
{
    string A(string str);
}

public class CTest : ITest
{
  	// 要拦截的函数必须是虚函数或者重写函数
    public virtual string A(string str)
    {
        Console.WriteLine(str);
        return str;
    }
}

// 构造拦截器
// 1.继承BaseInterceptor
// 2.重写OnCall(CallInfo info)函数
public class TestInterceptor : BaseInterceptor
{
		public override void OnCall(CallInfo info)
		{
				// DoSomething
        //在被拦截的方法执行完毕后 继续执行
				info.Invocation.Proceed();
				// DoSomething
		}
}

// Main函数
ServiceHost host = new ServiceHost(EnvironmentType.Develop);
host.RegisterInterceptor<TestInterceptor>();
host.RegisterType<CTest>().As<ITest>().AddInterfaceInterceptors(typeof(TestInterceptor));
var provider = host.Build();
var service = provider.GetService<ITest>();
service.A("Test");
```