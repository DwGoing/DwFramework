using System;
using System.Threading;

using SqlSugar;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Database;
using DwFramework.Database.Extensions;

namespace _Test.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceHost host = new ServiceHost(configFilePath: $"{AppDomain.CurrentDomain.BaseDirectory}Config.json");
                host.RegisterRepositories();
                host.InitService(provider =>
                {
                    var db = provider.GetDatabaseService().DbConnection;
                    db.Aop.OnLogExecuted = (sql, args) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(db.Ado.SqlExecutionTime.TotalMilliseconds + "ms");
                    };
                    var res = db.Union(db.Queryable<User>().Where(item => item.Name == "Test100"), db.Queryable<User>().Where(item => item.Name == "Test110")).ToArray();
                });
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    [SugarTable("user")]
    public class User
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
        public int ID { get; set; }
        [SugarColumn(ColumnName = "card_id")]
        public string CardId { get; set; }
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }
    }

    public class UserRepository : BaseRepository<User>
    {
        public UserRepository()
        {

        }
    }
}
