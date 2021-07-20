using System;
using System.Text;

namespace DwFramework.Core.Generator
{
    public static class RandomGenerator
    {
        /// <summary>
        /// 随机数生成器
        /// </summary>
        /// <returns></returns>
        public static Random GetRandom()
        {
            var seed = BitConverter.ToInt64(Guid.NewGuid().ToByteArray());
            return new Random((int)seed);
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int RandomNumber(int start, int end)
        {
            var random = GetRandom();
            return random.Next(start, end);
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double RandomNumber(double start, double end)
        {
            if (end - start <= 0) throw new Exception("end必须大于start");
            var random = GetRandom();
            return (end - start) * random.NextDouble() + start;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private const string CHARS = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string RandomString(int length)
        {
            var chars = CHARS.ToCharArray();
            var builder = new StringBuilder();
            for (int i = 0; i < length; i++) builder.Append(chars[RandomNumber(0, chars.Length)]);
            return builder.ToString();
        }
    }
}
