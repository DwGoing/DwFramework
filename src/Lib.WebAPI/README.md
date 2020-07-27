# DwFramework.WebAPI

```shell
PM> Install-Package DwFramework.WebAPI
或
> dotnet add package DwFramework.WebAPI
```

## DwFramework WebAPI库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "WebAPI": {
    "ContentRoot": "",
    "Listen": {
      "http": "0.0.0.0:10080"
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
ServiceHost host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
host.RegisterLog();
host.RegisterWebAPIService();
host.InitService(provider => provider.InitWebAPIServiceAsync<Startup>());
host.Run();
```

### 0x3 插件

引用DwFramework.Web.Plugins来使用Web插件

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