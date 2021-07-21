# DwFramework.Quartz

```shell
PM> Install-Package DwFramework.Quartz
或
> dotnet add package DwFramework.Quartz
```

## DwFramework Quartz封装库

### 0x1 注册服务及初始化

可以参考如下代码：

```c#
// 注册服务
host.RegisterTaskScheduleService();
```

### 0x2 基本使用方法

```c#
// 创建任务类
class NJob : ScheduleJob
{
    public override Task ExecuteAction()
    {
        // DoSomething
    }
}
// 创建调度器
service.CreateScheduler("Key");
// 创建任务
service.CreateJob<NJob>("Key", "0/5 * * * * ? ");
```