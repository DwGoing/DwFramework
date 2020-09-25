using System;
using Xunit;
using Xunit.Abstractions;

using DwFramework.Core;
using DwFramework.Database;

namespace _UnitTest
{
    public class Database
    {
        private readonly ITestOutputHelper _output;
        public Database(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Simple()
        {
            var host = new ServiceHost(configFilePath: "Config.json");
            host.RegisterDatabaseService();
            host.OnInitialized += provider =>
            {
                var service = provider.GetService<DatabaseService>();
            };
            host.Run();
        }
    }
}
