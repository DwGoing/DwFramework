using System.Text;
using System.Security.Cryptography;

using RSAExtensions;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    public static class EncryptUtil
    {
        public class Md5
        {
            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="str"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string Encode(string str, Encoding encoding = null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                byte[] bytes = encoding.GetBytes(str);
                using (var md5 = MD5.Create())
                {
                    byte[] encodedBytes = md5.ComputeHash(bytes);
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < encodedBytes.Length; i++)
                    {
                        builder.Append(encodedBytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
        }

        public class Aes
        {
            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="str"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static byte[] Encrypt(string str, string key, string iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                byte[] bytes = encoding.GetBytes(str);
                using (var aes = System.Security.Cryptography.Aes.Create())
                {
                    aes.Key = encoding.GetBytes(key);
                    aes.IV = encoding.GetBytes(iv);
                    aes.Mode = mode;
                    aes.Padding = padding;
                    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                }
            }

            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="str"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string EncryptToBase64(string str, string key, string iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null)
            {
                return Encrypt(str, key, iv, mode, padding, encoding).ToBase64String();
            }

            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="str"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string EncryptToHex(string str, string key, string iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null)
            {
                return Encrypt(str, key, iv, mode, padding, encoding).ToHex();
            }

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="str"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string Decrypt(byte[] bytes, string key, string iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                using (var aes = System.Security.Cryptography.Aes.Create())
                {
                    aes.Key = encoding.GetBytes(key);
                    aes.IV = encoding.GetBytes(iv);
                    aes.Mode = mode;
                    aes.Padding = padding;
                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    return encoding.GetString(decryptor.TransformFinalBlock(bytes, 0, bytes.Length));
                }
            }

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="base64String"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string DecryptFromBase64(string base64String, string key, string iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = System.Security.Cryptography.PaddingMode.PKCS7, Encoding encoding = null)
            {
                return Decrypt(base64String.FromBase64String(), key, iv, mode, padding, encoding);
            }

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="hexString"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="mode"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string DecryptFromHex(string hexString, string key, string iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7, Encoding encoding = null)
            {
                return Decrypt(hexString.FromHex(), key, iv, mode, padding, encoding);
            }
        }

        public class Rsa
        {
            /// <summary>
            /// 生成密钥对
            /// </summary>
            /// <param name="type"></param>
            /// <param name="size"></param>
            /// <param name="isPem"></param>
            /// <returns></returns>
            public static (string PrivateKey, string PublicKey) GenerateKeyPair(RSAKeyType type, int size = 1024, bool isPem = false)
            {
                using (var rsa = RSA.Create(size))
                {
                    return (rsa.ExportPrivateKey(type, isPem), rsa.ExportPublicKey(type, isPem));
                }
            }

            /// <summary>
            /// 公钥加密
            /// </summary>
            /// <param name="data"></param>
            /// <param name="type"></param>
            /// <param name="publicKey"></param>
            /// <param name="isPem"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static byte[] EncryptWithPublicKey(byte[] data, RSAKeyType type, string publicKey, bool isPem = false, RSAEncryptionPadding padding = null)
            {
                using (var rsa = RSA.Create())
                {
                    rsa.ImportPublicKey(type, publicKey, isPem);
                    return rsa.Encrypt(data, padding ?? RSAEncryptionPadding.Pkcs1);
                }
            }

            /// <summary>
            /// 公钥加密
            /// </summary>
            /// <param name="data"></param>
            /// <param name="type"></param>
            /// <param name="publicKey"></param>
            /// <param name="isPem"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string EncryptWithPublicKey(string data, RSAKeyType type, string publicKey, bool isPem = false, RSAEncryptionPadding padding = null, Encoding encoding = null)
            {
                if (encoding == null) encoding = Encoding.UTF8;
                return EncryptWithPublicKey(encoding.GetBytes(data), type, publicKey, isPem, padding).ToBase64String();
            }

            /// <summary>
            /// 私钥加密
            /// </summary>
            /// <param name="data"></param>
            /// <param name="type"></param>
            /// <param name="privateKey"></param>
            /// <param name="isPem"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static byte[] EncryptWithPrivateKey(byte[] data, RSAKeyType type, string privateKey, bool isPem = false, RSAEncryptionPadding padding = null)
            {
                using (var rsa = RSA.Create())
                {
                    rsa.ImportPrivateKey(type, privateKey, isPem);
                    return rsa.Encrypt(data, padding ?? RSAEncryptionPadding.Pkcs1);
                }
            }

            /// <summary>
            /// 私钥加密
            /// </summary>
            /// <param name="data"></param>
            /// <param name="type"></param>
            /// <param name="privateKey"></param>
            /// <param name="isPem"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string EncryptWithPrivateKey(string data, RSAKeyType type, string privateKey, bool isPem = false, RSAEncryptionPadding padding = null, Encoding encoding = null)
            {
                if (encoding == null) encoding = Encoding.UTF8;
                return EncryptWithPrivateKey(encoding.GetBytes(data), type, privateKey, isPem, padding).ToBase64String();
            }

            /// <summary>
            /// 私钥解密
            /// </summary>
            /// <param name="encryptedData"></param>
            /// <param name="type"></param>
            /// <param name="privateKey"></param>
            /// <param name="isPem"></param>
            /// <param name="padding"></param>
            /// <returns></returns>
            public static byte[] Decrypt(byte[] encryptedData, RSAKeyType type, string privateKey, bool isPem = false, RSAEncryptionPadding padding = null)
            {
                using (var rsa = RSA.Create())
                {
                    rsa.ImportPrivateKey(type, privateKey, isPem);
                    return rsa.Decrypt(encryptedData, padding ?? RSAEncryptionPadding.Pkcs1);
                }
            }

            /// <summary>
            /// 私钥解密
            /// </summary>
            /// <param name="encryptedData"></param>
            /// <param name="type"></param>
            /// <param name="privateKey"></param>
            /// <param name="isPem"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string Decrypt(string encryptedData, RSAKeyType type, string privateKey, bool isPem = false, RSAEncryptionPadding padding = null, Encoding encoding = null)
            {
                if (encoding == null) encoding = Encoding.UTF8;
                return encoding.GetString(Decrypt(encryptedData.FromBase64String(), type, privateKey, isPem, padding));
            }
        }
    }
}
