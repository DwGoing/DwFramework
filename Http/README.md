# DwFramework.Http

```shell
PM> Install-Package DwFramework.Http
或
> dotnet add package DwFramework.Http
```

## DwFramework WebAPI库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "Http": {
    "ContentRoot": "",
    "Listen": {
      "http": "0.0.0.0:10080"
    }
  }
}

```

### 0x2 注册服务及初始化

WebAPI服务的初始化和AspDotCore原生WebAPI的配置方法一致，可以直接用Startup类来封装，但需要继承BaseStartup基类。可以参考如下代码：

```c#
// Startup.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using DwFramework.Http;

namespace Test
{
    public class Startup : BaseStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
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
host.RegisterHttpService();
// 初始化
var provider = host.Build();
provider.InitHttpServiceAsync<Startup>();
```