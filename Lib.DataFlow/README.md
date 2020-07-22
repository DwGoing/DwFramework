# DwFramework.DataFlow

```shell
PM> Install-Package DwFramework.DataFlow
或
> dotnet add package DwFramework.DataFlow
```

## DwFramework 流式计算库

### 0x1 注册服务

可以参考如下代码：

```c#
host.RegisterDataFlowService();
```

### 0x2 创建任务队列

每一个任务队列生成一个唯一的key，之后的操作都需要通过这个key来操作任务队列。

每一个任务队列有两个处理器，ITaskHandler是将输入=>输出，IResultHandler是将输出+旧结果=>新结果。

```c#
var key = service.CreateTaskQueue(new TaskHandler(), new ResultHandler());

// 对输入的处理
public class TaskHandler : ITaskHandler<int, int>
{
    public int Invoke(int input)
    {
        return input * 10;
    }
}

// 对输出的处理
public class ResultHandler : IResultHandler<int, int>
{
    public int Invoke(int output, int result)
    {
        return output + result;
    }
}
```

### 0x3 配置队列

```c#
// 任务开始时
service.AddTaskStartHandler<int>(key, input => Console.WriteLine($"任务开始:{input}"));
// 任务结束时
service.AddTaskEndHandler<int, int, int>(key, (input, output, result) => Console.WriteLine($"任务结束:{input} {output} {result}"));
```

### 0x4 添加输入及获取结果

```c#
service.AddInput(key, 1);
service.AddInput(key, 2);
// 所有任务均为异步处理
// 获取的结果仅为某一个时刻的结果
Thread.Sleep(1000);
service.GetResult(key);
```