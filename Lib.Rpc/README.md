# DwFramework.Rpc

```shell
PM> Install-Package DwFramework.Rpc
或
> dotnet add package DwFramework.Rpc
```

## DwFramework Rpc库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "Rpc": {
    "Prefixes": [ "http://*:10010" ]
  }
}
```

### 0x2 注册服务及初始化

该库是整合了Hprose.RPC库的功能。

```c#
// 注册服务
host.RegisterRpcService();
// 初始化
var provider = host.Build();
provider.InitRpcServiceAsync();
```

### 0x3 注册Rpc函数

该库支持Hprose.RPC库原生的函数注册方式，可通过如下方式使用：

```c#
var rpc = provider.GetService<IRpcService, RpcService>();
// 函数原生注册
rpc.Service.Add(() => { Console.WriteLine("Hello World!")});
rpc.Service.Add<{返回类型}>(() => { Console.WriteLine("Hello World!")});
rpc.Service.AddMethod({函数名}, {目标实例});
// ...
```

也可以使用该库封装的函数注册方式，不过需要在函数定义时加上[Rpc]标签，可参考下列代码：

```c#
public interface ITestInterface
{
    void TestMethod(string str);
}

public class TestClass1 : ITestInterface
{
    public TestClass1()
    {
        Console.WriteLine("TestClass1已注入");
    }

  	// 若注册了多个相同接口的服务，可在此区分调用的函数
  	// 若不指定Rpc函数名称，则默认为函数签名名称
    [Rpc("Method1")]
    public void TestMethod(string str)
    {
        Console.WriteLine($"TestClass1:{str}");
    }
}

public class TestClass2 : ITestInterface
{
    public TestClass2()
    {
        Console.WriteLine("TestClass2已注入");
    }

    [Rpc("Method2")]
    public void TestMethod(string str)
    {
        Console.WriteLine($"TestClass2:{str}");
    }
}
```

```c#
// 从实例中注册Rpc函数
rpc.RegisterFuncFromInstance({实例}, {调用名称});
// 从服务中注册Rpc函数
rpc.RegisterFuncFromService<{接口类型},{实例类型}>();
rpc.RegisterFuncFromService<{接口类型}>();
```

其中需要特别注意的是，RegisterFuncFromService<{接口类型}>()会把实现相同接口的服务中所有Rpc函数都进行注册，如果在Rpc标签中对函数调用名称不进行区分，可能在调用时出现“调非所调”的情况。所以，我们建议当使用Rpc标签时尽量都标识调用名称。

### 0x4 函数调用

该库并为对Rpc客户端进行封装，你可以直接引用Hprose.RPC，使用其自带的客户端来调用函数。

```c#
var client = new Client("http://127.0.0.1:10010/");
client.Invoke("Method1", new object[] { "helo" });
client.Invoke("Method2", new object[] { "helo" });
```