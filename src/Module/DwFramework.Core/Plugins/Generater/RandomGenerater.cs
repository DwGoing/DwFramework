using System;
using System.Text;

namespace DwFramework.Core.Plugins
{
    public static class RandomGenerater
    {
        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <returns></returns>
        public static long RandomNumber() => BitConverter.ToInt64(Guid.NewGuid().ToByteArray());

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private const string CHARS = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string RandomString(int length)
        {
            char[] chars = CHARS.ToCharArray();
            Random random = new Random((int)RandomNumber());
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++) builder.Append(chars[random.Next(chars.Length)]);
            return builder.ToString();
        }
    }
}
