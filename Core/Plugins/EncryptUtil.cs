using System;
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
            /// 私钥加密
            /// </summary>
            /// <param name="str"></param>
            /// <param name="privateKey"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static byte[] EncryptWithPrivateKey(string str, string privateKey, RSAEncryptionPadding padding = null, Encoding encoding = null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                using (var rsa = RSA.Create())
                {
                    rsa.ImportRSAPrivateKey(privateKey.FromBase64String(), out _);
                    return rsa.Encrypt(encoding.GetBytes(str), padding ?? RSAEncryptionPadding.Pkcs1);
                }
            }

            /// <summary>
            /// 私钥加密
            /// </summary>
            /// <param name="str"></param>
            /// <param name="privateKey"></param>
            /// <param name="padding"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string EncryptWithPrivateKeyToBase64(string str, string privateKey, RSAEncryptionPadding padding = null, Encoding encoding = null)
            {
                return EncryptWithPrivateKey(str, privateKey, padding, encoding).ToBase64String();
            }

            public static byte[] EncryptWithPublicKey(string str, string publicKey, RSAEncryptionPadding padding = null, Encoding encoding = null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;
                using (var rsa = RSA.Create())
                {
                    rsa.ImportRSAPublicKey(publicKey.FromBase64String(), out _);
                    return rsa.Encrypt(encoding.GetBytes(str), padding ?? RSAEncryptionPadding.Pkcs1);
                }
            }

            public static string EncryptWithPublicKeyToBase64(string str, string privateKey, RSAEncryptionPadding padding = null, Encoding encoding = null)
            {
                return EncryptWithPublicKey(str, privateKey, padding, encoding).ToBase64String();
            }
        }
    }
}
