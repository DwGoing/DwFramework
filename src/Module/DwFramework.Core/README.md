# DwFramework.Core

```shell
PM> Install-Package DwFramework.Core
或
> dotnet add package DwFramework.Core
```

## DwFramework 核心库

### 0x1 快速开始

```c#
var host = new ServiceHost(); // 初始化服务主机
host.Register(context => new S1()); // 注册服务
host.RegisterType<S2>(); // 注册服务
host.RegisterFromAssemblies(); // 注册服务
host.OnInitialized += provider => provider.GetService<S1>().Do();
host.OnInitialized += provider => provider.GetService<S2>().Do();
host.OnInitialized += provider => provider.GetService<S3>().Do();
host.Run();

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
```

### 0x2 多种注册服务的方式

ServiceHost提供了多种方式的服务注册，尽可能地使注入方式更容易让人理解其内部实现。为了方便后面的案例说明，我们先定义示例中使用到的接口和类型。

##### 0x1 RegisterType<[实现类型]>

```c#
var host = new ServiceHost(); // 初始化服务主机
host.RegisterType<S>(); // 注册服务
host.OnInitialized += provider => provider.GetService<S>().Do(); // 获取服务
host.Run();
class S
{
    public void Do() => Console.WriteLine("s");
}
```

##### 0x2 RegisterType<[接口实现类型],[接口类型]>

```c#
var host = new ServiceHost(); // 初始化服务主机
host.RegisterType<S1, IS>(); // 注册服务
host.RegisterType<S2, IS>(); // 注册服务
host.OnInitialized += provider => provider.GetService<IS>().Do(); // 默认获取到的是最后注册的IS实现
host.Run();

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
```

##### 0x3 Register([注册函数])

```c#
var host = new ServiceHost(); // 初始化服务主机
host.Register(context => new S("hello")); // 注册服务
host.OnInitialized += provider => provider.GetService<S>().Do();
host.Run();

class S
{
    readonly string _tag;

    public S(string tag)
    {
        _tag = tag;
    }

    public void Do() => Console.WriteLine($"s_{_tag}");
}
```

##### 0x4 RegisterFromAssemblies

```c#
var host = new ServiceHost(); // 初始化服务主机
host.RegisterFromAssemblies(); // 注册服务
host.OnInitialized += provider => provider.GetService<IS>().Do(); // 默认获取到的是最后注册的IS实现
host.Run();

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
```

### 0x3 使用插件

##### 0x1 Aop

```c#
var host = new ServiceHost(); // 初始化服务主机
host.RegisterInterceptors(typeof(MyInterceptor)); // 注册拦截器
host.RegisterType<S>().AddClassInterceptors(typeof(MyInterceptor)); // 注册服务并添加拦截器
host.OnInitialized += provider => provider.GetService<S>().Do();
host.Run();

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
```

##### 0x2 NLog

```c#
// 注册Log组件
host.RegisterLog();
```

```xml
<!-- NLog.config示例 -->
<?xml version="1.0" encoding="utf-8"?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <extensions>
        <add assembly="NLog.MailKit" />
    </extensions>
    <targets>
        <!--使用可自定义的着色将日志消息写入控制台-->
        <target name="ColorConsole" xsi:type="ColoredConsole" layout="[${level}] ${date:format=yyyy\-MM\-dd HH\:mm\:ss}:${message} ${exception:format=message}" />
        <target name="Mail" xsi:type="Mail" smtpServer="smtp.mxhichina.com" smtpPort="465" smtpAuthentication="Basic" smtpUserName="账号" smtpPassword="密码" enableSsl="true" addNewLines="true" from="斑码网络&lt;bancode@bancode.net&gt;"
            to="***@***" subject="邮件主题" header="===============" body="${newline}${message}${newline}" footer="================" />
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

##### 0x3 MemoryCache

```c#
// 注册MemoryCache服务
// Hash容器数量默认为6，可根据主机配置设置
host.RegisterMemoryCache({Hash容器数量}, {是否为全局缓存}});

// 在实例中使用
[Registerable(typeof(A))]
public class A
{
    readonly ICache _cache;

    public A(ICache cache)
    {
        _cache = cache;
    }
  
    public void AddData()
    {
        var timer = new DwFramework.Core.Plugins.Timer();
        for (int i = 0; i < 1000000; i++)
        {
            // 插入数据
            _cache.Set(i.ToString(), i);
        }
        Console.WriteLine(timer.GetTotalMilliseconds() + "ms");
    }

    public void GetData()
    {
        // 获取数据
        Console.WriteLine(_cache.Get<int>("34986"));
    }
}
```

