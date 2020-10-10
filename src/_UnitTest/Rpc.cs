using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;
using DwFramework.Rpc;

namespace _UnitTest
{
    public class Rpc
    {
        [Fact]
        public void Simple()
        {
            var host = new ServiceHost(configFilePath: "Config.json");
            host.RegisterRpcService();
            host.Run();
        }
    }
}
