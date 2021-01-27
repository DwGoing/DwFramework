using System;

using DwFramework.Core;
using DwFramework.ORM;

namespace DwFramework.Example.ORM
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("ORM.json");
                host.RegisterORMService();
                host.OnInitialized += provider =>
                {
                    var ormService = provider.GetService<ORMService>();
                    var result = ormService.CreateConnection("db_sqlite").Queryable<Record>().ToArray();
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
