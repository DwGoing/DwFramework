using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.ORM;
using DwFramework.ORM.Plugins;
using SqlSugar;

namespace _AppTest
{
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
    [Repository]
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(string connName) : base(connName) { }
        /*
         * DoSomething
         */
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop, "Config.json");
                host.RegisterLog();
                host.RegisterORMService("ORM");
                host.RegisterRepositories();
                host.OnInitialized += p =>
                {
                    var s = p.GetORMService();
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}