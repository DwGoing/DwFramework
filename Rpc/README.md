# DwFramework.Rpc

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