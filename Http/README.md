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
namespace Test
{
    public class Startup : BaseStartup
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
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseJWTAuthentication();
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

### 0x3 插件

引用DwFramework.Http.Plugins来使用Http插件

1. JWT

```c#
// 验证器
public class TokenValidator : CustomSecurityTokenValidator
{
    public TokenValidator()
    {
        // 设置SecurityKey（必须）
        SetSecurityKeyValidation("jianghy1209inisinef1");
        // 设置Issuer（可选）
        SetIssuerValidation("");
        // 设置Audience（可选）
        SetAudienceValidation("");
    }

    public override bool ValidateToken(JwtSecurityToken token)
    {
        return true;
    }
}
```