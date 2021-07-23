using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.SqlSugar;

namespace SqlSugarExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new ServiceHost();
            // 可在运行时修改配置文件
            host.AddJsonConfiguration("Config.json", reloadOnChange: true);
            host.ConfigureSqlSugar();
            host.ConfigureLogging(builder => builder.UserNLog());
            host.OnHostStarted += p =>
            {
                var s = p.GetSqlSugarService();
                var c = s.CreateConnection("Main");
                var d = c.DbMaintenance.GetTableInfoList();
            };
            await host.RunAsync();
        }
    }
}
