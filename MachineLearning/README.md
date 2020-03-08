# DwFramework.MachineLearning

```shell
PM> Install-Package DwFramework.MachineLearning
或
> dotnet add package DwFramework.MachineLearning
```

## DwFramework MachineLearning库

### 0x1 注册服务及初始化

可以参考如下代码：

```c#
// 注册服务
host.RegisterMachineLearningService();
// 初始化
var provider = host.Build();
```