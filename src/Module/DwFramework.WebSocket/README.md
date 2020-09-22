# DwFramework.WebSocket

```shell
PM> Install-Package DwFramework.WebSocket
或
> dotnet add package DwFramework.WebSocket
```

## DwFramework WebSocket库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "WebSocket": {
    "ContentRoot": "",
    "Listen": {
      "ws": "0.0.0.0:10090"
    },
    "BufferSize": 100
  }
}
```

### 0x2 注册服务及初始化

```c#
ServiceHost host = new ServiceHost(configFilePath: $"Config.json");
host.RegisterLog();
host.RegisterWebSocketService();
host.OnInitializing += provider =>
{
    var service = provider.GetWebSocketService();
    service.OnConnect += (c, a) =>
    {
        Console.WriteLine($"{c.ID}已连接");
    };
    service.OnSend += (c, a) =>
    {
        Console.WriteLine($"向{c.ID}发送消息：{a.Message}");
    };
    service.OnReceive += (c, a) =>
    {
        Console.WriteLine($"收到{c.ID}发来的消息：{a.Message}");
    };
    service.OnClose += (c, a) =>
    {
        Console.WriteLine($"{c.ID}已断开");
    };
};
host.Run();
```

