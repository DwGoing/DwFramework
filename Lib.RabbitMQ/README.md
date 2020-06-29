# DwFramework.RabbitMQ

```shell
PM> Install-Package DwFramework.RabbitMQ
或
> dotnet add package DwFramework.RabbitMQ
```

## DwFramework RabbitMQ库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "RabbitMQ": {
    "Host": "主机地址",
    "VirtualHost": "/",
    "Port": 5672,
    "UserName": "admin",
    "Password": "admin"
  }
}

```

### 0x2 注册服务及初始化

可以参考如下代码：

```c#
// 注册服务
host.RegisterRabbitMQService();
```

### 0x3 基本使用方法

可通过如下方式使用：

```c#
// 创建交换机
service.ExchangeDeclare("ExchangeName", ExchangeType.Fanout);
// 创建队列
service.QueueDeclare("QueueName");
// 将队列绑定到交换机
service.QueueBind("QueueName", "ExchangeName");
// 订阅消息
service.Subscribe("QueueName", true, (sender,args) =>
	{
		// DoSomething
	});
// 发布消息
service.Publish("Msg", "ExchangeName");
// 取消订阅
service.Unsubscribe("QueueName");
```
