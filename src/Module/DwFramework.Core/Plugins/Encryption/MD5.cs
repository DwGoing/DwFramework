using System.Text;

namespace DwFramework.Core.Plugins
{
    public sealed class MD5
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
            using (var md5 = System.Security.Cryptography.MD5.Create())
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
}
