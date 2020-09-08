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
                var d = new string[10];
                d.ForEach(item =>
                {
                    item = Generater.GenerateGUID().ToString();
                    Console.WriteLine(item);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
