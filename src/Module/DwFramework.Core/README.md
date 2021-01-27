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

### 0x2 使用配置文件

提供两种配置方式，你可以将配置都写进全局配置文件中，或者单独对某个模块提供一个配置文件。后者是我们推荐的，每个模块我们会注明全局配置中的Key。

```json
//  全局配置
{
  "WebAPI": {
    "ContentRoot": "",
    "Listen": {
      "http": "0.0.0.0:10080"
    }
  }
}
// 模块配置
{
  "ContentRoot": "",
  "Listen": {
    "http": "0.0.0.0:10080"
  }
}
```

### 0x3 多种注册服务的方式

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

### 0x4 使用插件

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
var host = new ServiceHost(); // 初始化服务主机
host.RegisterLog(); // 注册服务
host.OnInitialized += provider =>
{
  var logger = provider.GetLogger<Example6>();
  logger.LogInformation("Example6"); // 添加日志
};
host.Run();
```

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

##### 0x3 MemoryCache

```c#
var host = new ServiceHost(); // 初始化服务主机
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
var keys = (PublicKey: @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDOW76CS+tvR0IYNld6K2JMHPmK
3zhLVrnqaiV58eR7GNtgfXUkf04hvuboLetdWI3K8qPIYEn1tcRLeOY/tn6cFTCq
lRb3XdfGiUtwTa+Nb76HJgWyufMEktPrOqKgbgn1ojdI53dillF/jwXJjpY+ddSa
gSCvJMc9vxc83mLQxwIDAQAB
-----END PUBLIC KEY-----", PrivateKey: @"-----BEGIN PRIVATE KEY-----
MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBAM5bvoJL629HQhg2
V3orYkwc+YrfOEtWuepqJXnx5HsY22B9dSR/TiG+5ugt611Yjcryo8hgSfW1xEt4
5j+2fpwVMKqVFvdd18aJS3BNr41vvocmBbK58wSS0+s6oqBuCfWiN0jnd2KWUX+P
BcmOlj511JqBIK8kxz2/FzzeYtDHAgMBAAECgYEAt1W5Fue+XtnvNbWp2EeNCFRB
vAh/aie9+y6c5w9qT5cQ6FPt7CQSVVbWrPaHAiK3rtQNgOtTKjJ4GBlsbrSDHC3t
evBLB+r7RZ4A7Z5TWdA73rXJBPRbbKSYV7PC41FiIXxmlXOQcfvbepbjmu5hyB5i
xYb3H9xWEfirEXY1g0kCQQDt6gCBaUWMuEAAHuF2vRVs7CMpj+LOdpJU5jqPWlyA
IwBsSTxUi+TY4RtXwGhzK7CZ1J3ZYw3G2rMx6IvAIUKrAkEA3gukpdyAVyFlWjpK
Zz+IwFBUuONQZk/LAe5AaB+6ImbR5ww3PTt6hS9lnel3YYqB5kaOELXAjQkLPaCu
XDcKVQJBAL2u0mZbIytVfxlZhZLgoCNuhX5OjJrlqDduM4Q1nAhBX8X2Ada6jmNn
3h/xdJVWYP/Up2E5ezNvDG2fJUSyf+8CQA/GY/wknjmSddDjM0YCjYScMGiyPZQH
NzT76Dd9iYvIIkF37LS89QdhRqbhX0nevTvO52jogLWEXvgR4lFK18ECQQDeBWVa
UMsDDblI3JUNUM9UForz9x5fFdo1aUegEF2qNpAoIisCEImRebxnHG34Ribvokld
owzUku++6SMSFh5x
-----END PRIVATE KEY-----");// Pem格式密钥
var rsa = RSA.EncryptWithPublicKey(str, RSAExtensions.RSAKeyType.Pkcs8, keys.PublicKey, true); // RSA
raw = RSA.Decrypt(rsa, RSAExtensions.RSAKeyType.Pkcs8, keys.PrivateKey, true);
```
##### 0x5 BloomFilter
```c#
var bloomFilter = new BloomFilter(10000000, 10);
for (var i = 0; i < 999999; i++)
{
    bloomFilter.Add(i);
}

var flag = bloomFilter.IsExist(999991);
```

##### 0x6 SnowflakeGenerater
```c#
var snowflakeGenerater = new SnowflakeGenerater(1, DateTime.Parse("2021.01.01"));
var id = snowflakeGenerater.GenerateId();
var snowflakeIdInfo = SnowflakeGenerater.DecodeId(id);
```