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
                var provider = host.Build();
                var service = provider.GetService<AccountRepository>();
                Console.WriteLine(service.FindAllAsync().Result.ToJson());
                while (true) Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    [SugarTable("account")]
    public class Account
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
        public int ID { get; set; }
        [SugarColumn(ColumnName = "create_time")]
        public DateTime CreateTime { get; set; }
        [SugarColumn(ColumnName = "user")]
        public string User { get; set; }
        [SugarColumn(ColumnName = "password")]
        public string Password { get; set; }
    }

    public class AccountRepository : BaseRepository<Account>
    {
        public AccountRepository(DatabaseService database) : base(database) { }
    }
}
