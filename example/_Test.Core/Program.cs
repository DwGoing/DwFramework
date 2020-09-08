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
                var d = new Dictionary<string, string>() { { "1", "a" }, { "2", "b" }, { "3", "c" } };
                d.ForEach(item => Console.WriteLine($"{item.Key} {item.Value}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
