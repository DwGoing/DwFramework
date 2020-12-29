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
                var gen = new SnowflakeGenerater(1, DateTime.Parse("2020.01.01"));
                var id = gen.GenerateId();
                Console.WriteLine(SnowflakeGenerater.DecodeId(id, DateTime.Parse("2020.01.01")).ToJson());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
