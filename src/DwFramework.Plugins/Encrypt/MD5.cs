using System;
using System.Text;

namespace DwFramework.Core.Encrypt
{
    public static class MD5
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encode(string str, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var bytes = encoding.GetBytes(str);
            using var md5 = System.Security.Cryptography.MD5.Create();
            var encodedBytes = md5.ComputeHash(bytes);
            var builder = new StringBuilder();
            for (int i = 0; i < encodedBytes.Length; i++) builder.Append(encodedBytes[i].ToString("x2"));
            return builder.ToString();
        }
    }
}