using RSAExtensions;

namespace DwFramework.Core.Plugins
{
    public static class KeyGenerater
    {
        /// <summary>
        /// 生成RSA密钥对
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="isPem"></param>
        /// <returns></returns>
        public static (string PrivateKey, string PublicKey) RsaKeyPair(RSAKeyType type = RSAKeyType.Pkcs1, int size = 1024, bool isPem = false)
        {
            using var rsa = System.Security.Cryptography.RSA.Create(size);
            return (rsa.ExportPrivateKey(type, isPem), rsa.ExportPublicKey(type, isPem));
        }
    }
}
