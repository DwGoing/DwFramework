# DwFramework.WebSocket

## DwFramework WebSocket库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```c#
{
  "WebSocket": {
    "ContentRoot": "",
    "Listen": {
      "ws": "0.0.0.0:10088"
    }
  }
}
```

### 0x2 注册服务及初始化

一般我们在服务初始化阶段，会读取配置文件。

```c#
// 注册WebSocket服务
host.RegisterWebSocketService();
// 初始化
var provider = host.Build();
provider.InitWebSocketServiceAsync(
    onConnect: (c, a) =>
    {
        Console.WriteLine($"{c.ID}已连接");
    },
    onSend: (c, a) =>
    {
        var msg = a.Message;
        Console.WriteLine($"向{c.ID}消息：{msg}");
    },
    onReceive: async(c, a) =>
    {
        var msg = a.Message;
        Console.WriteLine($"收到{c.ID}发来的消息：{msg}");
        if (msg == "close")
        {
            await c.WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
    },
    onClose: (c, a) =>
    {
        Console.WriteLine($"{c.ID}已断开");
    });
```
