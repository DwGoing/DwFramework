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
    [Description("a", "b")]
    enum X
    {
        [Description("x", "y")]
        A,
        B
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var a = typeof(X).GetField("A").GetDescription();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }
}
