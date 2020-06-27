using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;
using System.Text;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Save
                //var dataBytes = File.ReadAllBytes(@"C:\Users\DwGoing\Desktop\1.mp4");
                //var builder = new StringBuilder();
                //builder.Append($"mp4|{dataBytes.Length}|");
                //builder.Append(dataBytes.ToBase64String());
                //File.WriteAllText(@"C:\Users\DwGoing\Desktop\1", builder.ToString());
                // Road
                var fileText = File.ReadAllText(@"C:\Users\DwGoing\Desktop\1");
                var tmp = fileText.Split("|");
                File.WriteAllBytes($"C:\\Users\\DwGoing\\Desktop\\2.{tmp[0]}", tmp[2].FromBase64String());
                Console.WriteLine("Finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
