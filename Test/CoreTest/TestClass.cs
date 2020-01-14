using System;

using Microsoft.Extensions.Configuration;

using DwFramework.Core.Models;

namespace Test
{
    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass1 : ITestInterface
    {
        public class Config
        {
            public string A { get; set; }
            public string[] B { get; set; }
        }

        public TestClass1(IConfiguration configuration)
        {
            var config = configuration.GetSection("Test").Get<Config>();
            Console.WriteLine("TestClass1已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass1:{str}");
        }
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass2 : ITestInterface
    {
        public class Config
        {
            public string A { get; set; }
            public string[] B { get; set; }
        }

        public TestClass2(IConfiguration configuration)
        {
            var config = configuration.GetSection("Core").Get<Config>();
            Console.WriteLine("TestClass2已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass2:{str}");
        }
    }
}
