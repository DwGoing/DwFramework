using System;
using Xunit;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace _UnitTest
{
    public class Core
    {
        public Core()
        {

        }

        #region Extension
        private class TestClass : TestClassParent
        {
            public double C { get; set; }
        }

        private class TestClassParent
        {
            public string A { get; set; }
            public int B { get; set; }
        }

        [Fact]
        public void JsonSerialize()
        {
            var c = new TestClass() { A = "x", B = 5, C = 5.5 };
            var json = c.ToJson();
        }

        [Fact]
        public void Deserialize()
        {
            var json = "{\"A\":\"x\",\"B\":5,\"C\":5.5}";
            var c = json.ToObject<TestClass>();
        }
        #endregion

        #region Plugin
        [Fact]
        public void StopWatch()
        {
            Stopwatch.Static.SetStartTime();
            System.Threading.Thread.Sleep(3000);
            var s = Stopwatch.Static.GetTotalMilliseconds();
        }
        #endregion
    }
}
