# DwFramework.Plugins.Rpc

```shell
PM> Install-Package DwFramework.Plugins.Rpc
或
> dotnet add package DwFramework.Plugins.Rpc
```

## DwFramework Rpc插件库

### 0x1 Cluster

该插件基于RPC服务实现，满足服务实现去中心化的集群实现。

```json
{
  "LinkUrl": "{自身RPC IP:Port}",
  "BootPeer": "{启动连接节点}"
}
```

```c#
// 注册集群服务
host.RegisterClusterImpl({配置文件路径});
host.RegisterRpcService(); // 一定要在RegisterClusterImpl之后
host.OnInitializing += p =>
{
    var cluster = p.GetClusterImpl();
    cluster.OnJoin += id => Console.WriteLine($"欢迎 {id} 加入集群");
    cluster.OnExit += id => Console.WriteLine($"{id} 退出集群");
    // SyncData() 在集群中同步数据
    cluster.OnJoin += id => cluster.SyncData(DataType.Text, Encoding.UTF8.GetBytes($"欢迎 {id} 加入集群"));
    cluster.OnReceiveData += (id, type, data) => Console.WriteLine($"收到 {id} 消息:{data.ToObject<string>(Encoding.UTF8)}");
    cluster.OnConnectBootPeerFailed += ex => Console.WriteLine(ex);
};
```

