# DwFramework.Plugins.Rpc

```shell
PM> Install-Package DwFramework.Plugins.Rpc
或
> dotnet add package DwFramework.Plugins.Rpc
```

## DwFramework Rpc插件库

### 0x1 Cluster

该插件基于RPC服务实现，满足服务实现去中心化的集群实现。

```c#
// 注册集群服务
host.RegisterClusterImpl({本地服务连接URL}, {启动连接节点});
host.RegisterRpcService(); // 一定要在RegisterClusterImpl之后
host.OnInitializing += p =>
{
  var cluster = p.GetClusterImpl();
  // SyncData() 在集群中同步数据
  cluster.OnJoin += id => cluster.SyncData(Encoding.UTF8.GetBytes($"欢迎 {id} 加入集群"));
  cluster.OnReceiveData += (id, data) => Console.WriteLine($"收到 {id} 消息:{Encoding.UTF8.GetString(data)}");
};
```

