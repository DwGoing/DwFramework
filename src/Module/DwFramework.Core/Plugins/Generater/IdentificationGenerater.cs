using System;
using System.Text;

namespace DwFramework.Core.Plugins
{
    public static class IdentificationGenerater
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
        public static string RandomString(int length)
        {
            char[] chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            Random random = new Random((int)RandomNumber());
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++) builder.Append(chars[random.Next(chars.Length)]);
            return builder.ToString();
        }

        /// <summary>
        /// 生成流水号
        /// [日期 8位][时间 4位][随机数 8位][Nonce 4位][自定义标识 n位]
        /// </summary>
        /// <param name="customNum"></param>
        /// <returns></returns>
        private static readonly object _uuidLock = new object();
        private static int _uuidNonce = 0;
        private static string _lastTimeTag = DateTime.Now.ToString("HHmmss");
        public static string UUID(string customTag = null)
        {
            StringBuilder builder = new StringBuilder();
            DateTime nowTime = DateTime.Now;
            // 日期+时间
            builder.Append(nowTime.ToString("yyyyMMddHHmm"));
            Random random = new Random((int)RandomNumber());
            // 随机数
            builder.Append(random.Next(100000000).ToString().PadLeft(8, '0'));
            // Nonce
            lock (_uuidLock)
            {
                if (_lastTimeTag == nowTime.ToString("HHmmss"))
                {
                    _uuidNonce++;
                }
                else
                {
                    _lastTimeTag = nowTime.ToString("HHmmss");
                    _uuidNonce = 0;
                }
            }
            builder.Append(_uuidNonce.ToString().PadLeft(4, '0'));
            // 自定义标识
            if (customTag != null) builder.Append(customTag);
            Console.Write($"{nowTime}");
            return builder.ToString();
        }
    }
}
