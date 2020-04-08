using System;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace _Test.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost();
            host.RegisterType<ITest, CTest>();
            var provider = host.Build();
            provider.GetService<ITest>().M();
            Console.Read();
        }
    }

    public interface ITest
    {
        void M();
    }

    public class CTest : ITest
    {
        public void M()
        {
            Console.WriteLine("Helo");
        }
    }
}
