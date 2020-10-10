# DwFramework.Plugins.WebAPI

```shell
PM> Install-Package DwFramework.Plugins.WebAPI
或
> dotnet add package DwFramework.Plugins.WebAPI
```

## DwFramework WebAPI插件库

### 0x1 Swagger

```c#
// ConfigureServices
services.AddSwagger("Doc", "Test", "v1");

// Configure
app.UseSwagger("Doc", "My API V1");
```

### 0x2 RequestFilter

```c#
// Configure
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

### 0x3 Consul

```json
{
  "Consul": {
    "Host": "http://127.0.0.1:8500", // Consul服务端IP及端口
    "ServiceHost": "127.0.0.1", // 服务IP
    "HealthCheckPort": 10080, // 健康检查端口
    "Services": [
      {
        "Name": "WebAPIService", // 服务名称
        "Port": 10080 // 服务端口
      }
    ]
  }
}
```

```c#
// Configure
app.UseConsul(lifetime, ServiceHost.Environment.GetConfiguration(), "Consul");
```

### 0x4 Jwt

```c#
// ConfigureServices
services.AddJwtAuthentication(new DefaultJwtTokenValidator("fc3d06d9b75f92b648ab4e372dfd22f2"), context =>
{
	Console.WriteLine("Success");
	return Task.CompletedTask;
}, context =>
{
	Console.WriteLine("Fail");
	return Task.CompletedTask;
});
// Configure
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization(); // 一定要在UseRouting和UseEndpoints之间
app.UseEndpoints(endpoints =>{endpoints.MapControllers();});
```