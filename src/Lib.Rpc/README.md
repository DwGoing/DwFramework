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
[Rpc(typeof(A))]
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