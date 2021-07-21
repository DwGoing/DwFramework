# DwFramework.SqlSugar

```shell
PM> Install-Package DwFramework.SqlSugar
或
> dotnet add package DwFramework.SqlSugar
```

## DwFramework SqlSugar封装库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
{
  "ConnectionString": "", // 连接字符串
  "DbType": "", // 数据库类型
  // 主从模式（可选）
  "SlaveConnections": [
    {
      "ConnectionString": "", // 连接字符串
      "HitRate": "" // 命中率
    }
  ],
  "UseMemoryCache": "" // 使用缓存
}
```

### 0x2 注册服务

```c#
// 配置服务
host.ConfigureSqlSugarWithJson("Config.json");
```

### 0x3 简单使用

```c#
// 定义实体
[SugarTable("record")]
public class User
{
    [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
    public int ID { get; set; }
    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }
    [SugarColumn(ColumnName = "is_enable")]
    public int IsEnable { get; set; }
}

// 获取SqlSugar服务进行数据库操作
var db = provider.GetService<SqlSugarService>();
var result = db.CreateConnection("db").Queryable<Record>().ToArray();
```

更多的操作实现可以参考SqlSugar文档示例。