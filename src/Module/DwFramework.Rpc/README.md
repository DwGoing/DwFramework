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
    "Listen": {
      "http": "localhost:5000"
    }
  }
}
```

### 0x2 注册服务及初始化

```c#
// 注册服务
host.RegisterRpcService();
// 初始化服务
host.InitService(provider => provider.InitRpcService());
// 对于非自定义的RPC服务类可以通过手动注册（无法使用[RPC]特性标签）
host.InitService(provider => {
  var rpc = provider.GetRpcService();
  rpc.AddService(provider.GetService<AService>());
});
```

### 0x3 注册Rpc函数

该库是基于gRPC实现的，首先需要编写.proto文件来生成服务端及客户端代码，如下：

```protobuf
syntax = "proto3";

service A {
  rpc Do (Request) returns (Response);
}

message Request {
  string message = 1;
}

message Response {
  string message = 1;
}

```

如果使用VS IDE，可引用Google.Protobuf、Grpc.Tools、Grpc.Core包来生成代码A.cs、AGrpc.cs。需要注意的是，生成时注意配置生成Server还是Client。

```c#
// a.proto的实现类
// 需要在host中注册
// host.RegisterType<AService>();
[Rpc]
public class AService : A.ABase
{
  public override Task<Response> Do(Request request, ServerCallContext context)
  {
    return Task.FromResult(new Response()
    {
      Message = request.Message
    });
  }
}
```

### 0x4 函数调用

可通过gRPC常规调用方法来调用，不同语言的调用可参考：https://github.com/grpc/grpc

```c#
var channel = new Channel("localhost:5000", ChannelCredentials.Insecure);
var client = new A.AClient(channel);
Console.WriteLine(client.Do(new Request() { Message = "123" }).Message);
channel.ShutdownAsync();
```

### 0x5 集群插件 

该插件基于RPC服务实现，满足服务实现去中心化的集群实现。

```c#
// 注册集群服务
host.RegisterClusterImpl({本地服务连接URL}, {启动连接节点});
host.RegisterRpcService();
host.OnInitializing += p =>
{
  var cluster = p.GetClusterImpl();
  // SyncData() 在集群中同步数据
  cluster.OnJoin += id => cluster.SyncData(Encoding.UTF8.GetBytes($"欢迎 {id} 加入集群"));
  cluster.OnReceiveData += (id, data) => Console.WriteLine($"收到 {id} 消息:{Encoding.UTF8.GetString(data)}");
};
```