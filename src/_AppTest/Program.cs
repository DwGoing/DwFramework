using System;
using System.Threading.Tasks;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using DwFramework.Media;

using OpenCvSharp;

namespace _AppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var a = 1;
                var f = new BloomFilter();
                f.Add(a);
                Console.WriteLine(f.IsExist(2));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
