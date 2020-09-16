using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace _UnitTest
{
    public class Core
    {
        private readonly ITestOutputHelper _output;
        public Core(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Tmp()
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
        public void JsonDeserialize()
        {
            var json = "{\"A\":\"x\",\"B\":5,\"C\":5.5}";
            var c = json.ToObject<TestClass>();
        }

        [Fact]
        public void GetPinYin()
        {
            '将'.GetPinYin();
        }

        [Fact]
        public void IsEmailAddress()
        {
            var str = "jianghy1209@163.com";
            Assert.True(str.IsEmailAddress());
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
