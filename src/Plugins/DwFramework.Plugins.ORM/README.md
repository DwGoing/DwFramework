# DwFramework.Plugins.Database

```shell
PM> Install-Package DwFramework.Plugins.Database
或
> dotnet add package DwFramework.Plugins.Database
```

## DwFramework ORM插件库

### 0x1 使用仓储

该库支持提供仓储模版以简化开发，可通过如下方式使用：

```c#
using SqlSugar;
// 定义实体
public class User
{
    [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
    public int ID { get; set; }
    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }
    [SugarColumn(ColumnName = "is_enable")]
    public int IsEnable { get; set; }
}
// 定义仓储模型
public class UserRepository : BaseRepository<User>
{
    public UserRepository(DatabaseService databaseService) : base(databaseService) { }
    /*
     * DoSomething
     */
}
// 注册所有仓储
host.RegisterRepositories();
// 获取仓储
var repository = provider.GetService<UserRepository>();
```

仓储接口已定义了以下常规的增删改查操作：

```c#
Task<T[]> FindAllAsync();
Task<T[]> FindAsync(Expression<Func<T, bool>> expression);
Task<T> FindSingleAsync(Expression<Func<T, bool>> expression);
Task<T> InsertAsync(T newRecord);
Task<int> InsertAsync(T[] newRecords);
Task<int> DeleteAsync(Expression<Func<T, bool>> expression);
Task<bool> UpdateAsync(T newRecord);
Task<int> UpdateAsync(T[] newRecords);
```

若需要自定义操作或者使用更底层的数据库操作，可以通过基类中的Db来实现。如下面的代码可以实现事务操作：

```c#
public class UserRepository : BaseRepository<User>
{
    public UserRepository(DatabaseService databaseService) : base(databaseService) { }
    public void DoSomething() {
      	Db.Ado.UseTran(() =>
        {
            /*
             * DoSomething
             */
        });
    }
}
```
