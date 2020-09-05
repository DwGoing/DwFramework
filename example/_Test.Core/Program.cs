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
    abstract class A
    {
        public abstract string x { get; }
    }

    class B : A
    {
        public override string x => "a";
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var s = new B().ToJson();
                var x = s.ToObject<A>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }
}
