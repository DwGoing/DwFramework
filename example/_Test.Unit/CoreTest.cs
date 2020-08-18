using Microsoft.VisualStudio.TestTools.UnitTesting;

using DwFramework.Core.Plugins;

namespace _Test.Unit
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void Md5Encode()
        {
            var str = "oijnidvnnehowierjnw3209j45;390j9[23";
            var encodedStr = MD5.Encode(str);
            Assert.AreEqual(encodedStr, "ab15f6a56be7c3a457e8001ca0c8bc99");
        }
    }
}
