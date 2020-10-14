using System;
using System.Threading;
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

        #region Extension
        [Fact]
        public void RegisterFromAssembly()
        {
            var host = new ServiceHost();
            host.RegisterFromAssemblies();
        }

        [Serializable]
        private class TestClass : TestClassParent
        {
            public double C { get; set; }
        }

        [Serializable]
        private class TestClassParent
        {
            public string A { get; set; }
            public int B { get; set; }
        }

        [Fact]
        public void Json()
        {
            var a = new TestClass() { A = "x", B = 5, C = 5.5 };
            var json = a.ToBytes().ToBase64String();
            var obj = json.FromBase64String().ToObject<TestClass>();
        }

        [Fact]
        public void JsonSerializeAndDeserialize()
        {
            var a = new TestClass() { A = "x", B = 5, C = 5.5 };
            var json = a.ToJson();
            Assert.Equal(a, json.ToObject<TestClass>());
        }

        [Fact]
        public void GetPinYin()
        {
            Assert.Equal("HAO", '好'.GetPinYin()[0]);
        }

        [Fact]
        public void IsEmailAddress()
        {
            var str = "jianghy1209@163.com";
            Assert.True(str.IsEmailAddress());
        }

        [Fact]
        public void ExpressionOr()
        {
            Expression<Func<int, bool>> a = i => i != 5;
            Expression<Func<int, bool>> b = i => i > 1;
            var c = a.Or(b);
            Assert.True(c.Compile()(5));
        }
        #endregion

        #region Plugin
        [Fact]
        public void StopWatch()
        {
            Stopwatch.Static.SetStartTime();
            System.Threading.Thread.Sleep(3000);
            Assert.Equal(3000, Stopwatch.Static.GetTotalMilliseconds());
        }

        [Fact]
        public void MemoryCache()
        {
            ICache m = new MemoryCache(6);
            var a = new { A = "1", B = 2 };
            m.Set("test", a); // 插入数据
            Assert.Equal(a, m.Get("test")); // 获取数据
        }

        [Fact]
        public void RSAEncryptAndDecrypt()
        {
            var keys = KeyGenerater.RsaKeyPair(RSAExtensions.RSAKeyType.Pkcs8, isPem: true);
            var str = "DwFramework";
            var rsa = RSA.EncryptWithPublicKey(str, RSAExtensions.RSAKeyType.Pkcs8, keys.PublicKey, true);
            Assert.Equal(str, RSA.Decrypt(rsa, RSAExtensions.RSAKeyType.Pkcs8, keys.PrivateKey, true));
        }

        [Fact]
        public void CreateTask()
        {
            TaskManager.CreateTask(token =>
            {
                var i = 0;
                while (!token.IsCancellationRequested)
                {
                    i++;
                    _output.WriteLine(i.ToString());
                    Thread.Sleep(1000);
                }
            }, 10000);
            Console.ReadLine();
        }

        [Fact]
        public void TimeoutPolicy()
        {
            try
            {
                PolicyManager.Timeout(5000).Execute(() => Thread.Sleep(10000));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
