using System.Security.Cryptography;
using System.Text;

using DwFramework.Extensions.Core;

namespace DwFramework.Plugins.Core
{
    public class AES
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
            using (var aes = Aes.Create())
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
}
