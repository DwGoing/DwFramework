using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using SqlSugar;

namespace _AppTest
{
    [SugarTable("user")]
    public sealed class User
    {
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true)]
        public long ID { get; set; }
        [SugarColumn(ColumnName = "tag")]
        public string Tag { get; set; }
        [SugarColumn(ColumnName = "address")]
        public string Address { get; set; }
    }

    [Registerable(lifetime: Lifetime.Singleton, isAutoActivate: true)]
    public sealed class R : BaseRepository<User>
    {
        public R(ORMService ormService) : base(ormService, "MySql") { }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("Config.json");
                host.RegisterLog();
                host.RegisterORMService("ORM");
                host.RegisterFromAssemblies();
                host.OnInitializing += p =>
                {
                };
                host.OnInitialized += p =>
                {
                    var s = p.GetService<R>();
                    var res = s.InsertOrUpdateAsync(new List<User>(){
                        new User(){ Tag = "TEST1" , Address = "0xf22Cc2f4Ba4fE5Ab7e23973FfA9dA816cff0E13D"}
                    }, item => item.Address).Result;
                };
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }
        }
    }
}

