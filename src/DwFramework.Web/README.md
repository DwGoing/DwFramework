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
// WebAPI
{
  "ContentRoot": "",
  "Listen": {
    "http": "0.0.0.0:10080"
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
host.RegisterWebAPIService<Startup>();
host.Run();
```
