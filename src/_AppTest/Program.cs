using System;
using System.Threading.Tasks;
using DwFramework.Core;
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
                Console.WriteLine(DwFramework.Core.Plugins.RandomGenerater.RandomNumber(3.1, 6.7));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
