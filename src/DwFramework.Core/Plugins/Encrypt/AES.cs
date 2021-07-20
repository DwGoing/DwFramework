using System;
using System.Security.Cryptography;

namespace DwFramework.Core.Encrypt
{
    public static class AES
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="mode"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            using var encoder = Aes.Create();
            if (encoder.Key.Length != key.Length) throw new Exception("Key长度错误");
            encoder.Key = key;
            if (encoder.IV.Length != iv.Length) throw new Exception("IV长度错误");
            encoder.IV = iv;
            encoder.Mode = mode;
            encoder.Padding = padding;
            using var encryptor = encoder.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="mode"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv, CipherMode mode = CipherMode.ECB, PaddingMode padding = PaddingMode.PKCS7)
        {
            using var encoder = Aes.Create();
            if (encoder.Key.Length != key.Length) throw new Exception("Key长度错误");
            encoder.Key = key;
            if (encoder.IV.Length != iv.Length) throw new Exception("IV长度错误");
            encoder.IV = iv;
            encoder.Mode = mode;
            encoder.Padding = padding;
            using var decryptor = encoder.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }
    }
}