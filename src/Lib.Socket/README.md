# DwFramework.Socket

```shell
PM> Install-Package DwFramework.Socket
或
> dotnet add package DwFramework.Socket
```

## DwFramework Socket库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "Socket": {
    "Listen": "0.0.0.0:10100"
  }
}
```

### 0x2 注册服务及初始化

```c#
// 注册服务
host.RegisterSocketService();
host.InitService(provider =>
{
  provider.InitSocketServiceAsync();
  var service = provider.GetSocketService();
  service.OnConnect += (c, a) =>
  {
    Console.WriteLine($"{c.ID}已连接");
  };
  service.OnSend += (c, a) =>
  {
    Console.WriteLine($"向{c.ID}消息：{Encoding.UTF8.GetString(a.Data)}");
  };
  service.OnReceive += (c, a) =>
  {
    Console.WriteLine($"收到{c.ID}发来的消息：{Encoding.UTF8.GetString(a.Data)}");
    var data = new { A = "a", B = 123 }.ToJson();
    var msg = $"HTTP/1.1 200 OK\r\nContent-Type:application/json;charset=UTF-8\r\nContent-Length:{data.Length}\r\nConnection:close\r\n\r\n{data}";
    service.SendAsync(c.ID, msg);
  };
  service.OnClose += (c, a) =>
  {
    Console.WriteLine($"{c.ID}已断开");
  };
});
host.Run();
```
