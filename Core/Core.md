# DwFramework.Core

DwFramework核心库

```c#
ServiceHost host = new ServiceHost();
// 注册配置文件
host.RegisterConfiguration($"{Directory.GetCurrentDirectory()}/CoreTest", "Config.json");
```

```c#
host.RegisterFromAssembly("{程序集名}"); // 从程序集注入
host.RegisterInstance({类的实现}); // 某个类实现的注入
```