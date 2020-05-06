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
    "ContentRoot": "",
    "HttpListen": {
      "http": "0.0.0.0:10080"
    },
    "WebSocketListen": {
      "ws": "0.0.0.0:10088"
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
          	// JWT插件
            services.AddJWTAuthentication(new TokenValidator(), context =>
                {
                    Console.WriteLine("Success");
                    return Task.CompletedTask;
                }, context =>
                {
                    Console.WriteLine("Fail");
                    return Task.CompletedTask;
                });
            services.AddControllers();
            services.AddSwagger("v1", "Test", "v1");
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseJWTAuthentication(); // 必须在UseRouting之后
            app.UseSwagger("/swagger/v1/swagger.json", "My API V1");
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
host.RegisterWebService();
// 初始化
var provider = host.Build();
provider.InitHttpServiceAsync<Startup>();
provider.InitWebSocketServiceAsync(
	onConnect: (c, a) =>
	{
		Console.WriteLine($"{c.ID}已连接");
	},
	onSend: (c, a) =>
	{
		Console.WriteLine($"向{c.ID}消息：{a.Message}");
	},
	onReceive: (c, a) =>
	{
		Console.WriteLine($"收到{c.ID}发来的消息：{a.Message}");
	},
	onClose: (c, a) =>
	{
		Console.WriteLine($"{c.ID}已断开");
	}
);
```

### 0x3 插件

引用DwFramework.Http.Plugins来使用Http插件

1. JWT

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // JWT插件
        // 默认验证器
        services.AddJwtAuthentication(new JwtManager.DefaultJwtTokenValidator("fc3d06d9b75f92b648ab4e372dfd22f2"), context =>
        {
            Console.WriteLine("Success");
            return Task.CompletedTask;
        }, context =>
        {
            Console.WriteLine("Fail");
            return Task.CompletedTask;
        });
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseJwtAuthentication(); // 必须在UseRouting之后
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

2. Swagger

只需在Startup中注入服务即可。

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwagger("v1", "Test", "v1");
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseSwagger("/swagger/v1/swagger.json", "My API V1");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```