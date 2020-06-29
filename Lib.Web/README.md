# DwFramework.Web

```shell
PM> Install-Package DwFramework.Web
或
> dotnet add package DwFramework.Web
```

## DwFramework Web库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "Web": {
    "Http": {
      "ContentRoot": "",
      "Listen": {
        "http": "0.0.0.0:10080"
      }
    },
    "WebSocket": {
      "ContentRoot": "",
      "Listen": {
        "ws": "0.0.0.0:10090"
      },
      "BufferSize": 100
    },
    "Socket": {
      "Listen": "0.0.0.0:10100"
    }
  }
}
```

### 0x2 注册服务及初始化

WebAPI服务的初始化和AspDotCore原生WebAPI的配置方法一致，可以直接用Startup类来封装。可以参考如下代码：

```c#
// Startup.cs
namespace Test
{
    public class Startup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwagger("Doc", "Test", "v1");
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseSwagger("Doc", "My API V1");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
```

```c#
// 注册服务
host.RegisterWebService<HttpService>();
host.RegisterWebService<WebSocketService>();
host.InitService(provider => provider.InitHttpServiceAsync<Startup>());
host.InitService(provider =>
{
    provider.InitWebSocketServiceAsync();
    var service = provider.GetWebService<WebSocketService>();
    service.OnConnect += (c, a) =>
    {
        Console.WriteLine($"{c.ID}已连接");
    };
    service.OnSend += (c, a) =>
    {
        Console.WriteLine($"向{c.ID}消息：{a.Message}");
    };
    service.OnReceive += (c, a) =>
    {
        Console.WriteLine($"收到{c.ID}发来的消息：{a.Message}");
    };
    service.OnClose += (c, a) =>
    {
        Console.WriteLine($"{c.ID}已断开");
    };
});
host.Run();
```

### 0x3 插件

引用DwFramework.Http.Plugins来使用Http插件

1. Swagger

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwagger("Doc", "Test", "v1");
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseSwagger("Doc", "My API V1");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

2. RequestFilter 请求过滤器

```c#
app.UseRequestFilter(new Dictionary<string, Action<HttpContext>>
{
    {"/*",context =>{
        // 请求日志
        _logger.LogInformation($"接收到请求:{context.Request.Path} ({GetIP(context)})");
        // 自定义类型预处理
        CustomContentTypeHandler(context);
    }}
});
```