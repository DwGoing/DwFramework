using System;
using System.Text;

namespace DwFramework.Core.Encrypt
{
    public static class SHA256
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encrypt(byte[] data)
        {
            using var encoder = System.Security.Cryptography.SHA256.Create();
            var bytes = encoder.ComputeHash(data, 0, data.Length);
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
            return builder.ToString();
        }
    }
}