# DwFramework.ORM

```shell
PM> Install-Package DwFramework.ORM
或
> dotnet add package DwFramework.ORM
```

## DwFramework ORM库

### 0x1 配置

当使用该库时，需提前读取配置文件，Json配置如下：

```json
// ORM
{
  "ConnectionConfigs": {
    //定义多数据库配置
    "db_mysql": {
      "ConnectionString": "", // 连接字符串
      "DbType": "MySql", // 数据库类型
      // 主从模式（可选）
      "SlaveConnections": [
        {
          "ConnectionString": "", // 连接字符串
          "HitRate": "" // 命中率
        }
      ],
      "UseMemoryCache": true
    },
    "db_sqlite": {
      "ConnectionString": "Data Source=Record.db;",
      "DbType": "SqLite"
    }
  }
}
```

### 0x2 注册服务

```c#
// 注册服务
host.RegisterORMService();
```

### 0x3 简单使用

```c#
// 定义实体
[SugarTable("record")]
public class Record
{
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
    public long ID { get; set; }
    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }
}

// 获取ORM服务进行数据库操作
var host = new ServiceHost();
host.AddJsonConfig("ORM.json");
host.RegisterORMService();
host.OnInitialized += provider =>
{
    var ormService = provider.GetService<ORMService>();
    var result = ormService.CreateConnection("db_sqlite").Queryable<Record>()
    .ToArray();
};
host.Run();
```

更多的操作实现可以参考SqlSugar文档示例。