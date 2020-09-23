using System;

using DwFramework.Core;
using DwFramework.Database;

namespace DwFramework.Example.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost();
                host.AddJsonConfig("Database.json");
                host.RegisterDatabaseService();
                host.OnInitialized += provider =>
                {
                    var db = provider.GetService<DatabaseService>();
                    var result = db.DbConnection.Queryable<Record>().ToArray();
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
