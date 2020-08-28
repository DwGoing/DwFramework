using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = new ServiceHost(EnvironmentType.Develop);
                host.RegisterFromAssemblies();
                host.OnInitializing += p =>
                {
                    var c = p.GetService<CTest>();
                };
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }

    [Registerable(isAutoActivate: true)]
    public class CTest : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
