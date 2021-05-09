# DwFramework.Core

```shell
PM> Install-Package DwFramework.Core
或
> dotnet add package DwFramework.Core
```

## DwFramework 核心库

### 0x1 快速开始

```c#
// 定义接口
public interface ITestInterface
{
    void TestMethod(string str);
}

// 定义实现
[Registerable]
public class TestClass : ITestInterface
{
    public void TestMethod(string str)
    {
        Console.WriteLine($"TestClass:{str}");
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
            p.GetService<TestClass>().TestMethod("Hi");
        };
        await host.RunAsync();
    }
}
```

### 0x2 使用配置文件

提供多种配置方式，你可以将配置都写进同一个配置文件中，使用时通过指定路径获取对应配置；或者单独对某个模块创建一个配置文件。后者是我们推荐的。

```json
{
  "ID": 1,
  "Name": "XXX"
}
```

```c#
class Config
{
    public int ID { get; set; }
    public string Name { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.AddJsonConfig("Config.json");
        host.OnInitialized += p =>
        {
            var config = ServiceHost.Environment.GetConfiguration<Config>();
        };
        await host.RunAsync();
    }
}
```

### 0x3 多种注册服务的方式

ServiceHost提供了多种方式的服务注册，尽可能地使注入方式更容易让人理解其内部实现。为了方便后面的案例说明，我们先定义示例中使用到的接口和类型。

```c#
host.RegisterType<TestClass, ITestInterface>(); // 手动注入无参构造函数
host.Register<ITestInterface>(_ => new TestClass()); // 手动注入有依赖的构造函数
host.RegisterFromAssemblies(); // 从程序集注入（配合Registerable特性）
```

### 0x4 使用插件

##### 0x1 Aop

```c#
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
```

##### 0x2 NLog

```xml
<!-- NLog.config示例 -->
<?xml version="1.0" encoding="utf-8"?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <extensions>
        <add assembly="NLog.MailKit" />
        <add assembly="NLog.Targets.ElasticSearch" />
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
        <target name="Elk" xsi:type="ElasticSearch" index="test" uri="http://192.168.10.100:9200" requireAuth="false">
            <field name="Level" layout="${level}" />
            <field name="Host" layout="${machinename}" />
            <field name="Time" layout="${longdate}" />
            <field name="Message" layout="${message}" />
        </target>
    </targets>
    <!--规则配置,final - 最终规则匹配后不处理任何规则-->
    <rules>
        <logger name="*" minlevel="Debug" writeTo="ColorConsole" />
        <logger name="*" minlevel="Debug" writeTo="Elk" />
        <logger name="*" minlevel="Info" writeTo="AsyncFile" />
        <logger name="*" minlevel="Error" writeTo="Mail" />
        <logger name="Microsoft.*" minlevel="Info" writeTo="" final="true" />
    </rules>
</nlog>
```

```c#
host.RegisterLog(); // 注册服务
host.OnInitialized += provider =>
{
  var logger = provider.GetLogger<Example6>();
  logger.LogInformation("Example6"); // 添加日志
};
host.Run();
```

##### 0x3 MemoryCache

```c#
host.RegisterMemoryCache(); // 注册服务
host.OnInitialized += provider =>
{
  var cache = provider.GetCache();
  cache.Set("test", new { A = "1", B = 2 }); // 插入数据
  var value = cache.Get("test"); // 获取数据
};
host.Run();
```

##### 0x4 Encryption

```c#
var str = "DwFramework";
var md5 = MD5.Encode(str); // MD5
var aes = AES.EncryptToHex(str, "1234567890abcdef", "1234567890abcdef"); // AES
var raw = AES.DecryptFromHex(aes, "1234567890abcdef", "1234567890abcdef");
var keys = RSA.GenerateKeyPair(RSAExtensions.RSAKeyType.Pkcs8, isPem: true); // Pem格式密钥
var rsa = RSA.EncryptWithPublicKey(str, RSAExtensions.RSAKeyType.Pkcs8, keys.PublicKey, true); // RSA
raw = RSA.Decrypt(rsa, RSAExtensions.RSAKeyType.Pkcs8, keys.PrivateKey, true);
```