using System.Text;

namespace DwFramework.Core.Encrypt
{
    public static class MD5
    {
        /// <summary>
        /// 加密（32位）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encrypt(byte[] data)
        {
            using var encoder = System.Security.Cryptography.MD5.Create();
            var bytes = encoder.ComputeHash(data);
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
            return builder.ToString();
        }
    }
}