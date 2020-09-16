using System;
using System.Text;

namespace DwFramework.Core.Plugins
{
    public static class Generater
    {
        /// <summary>
        /// 生成GUID
        /// </summary>
        /// <returns></returns>
        public static Guid GenerateGUID() => Guid.NewGuid();

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            char[] chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            Random random = new Random(DateTime.Now.Millisecond);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++) builder.Append(chars[random.Next(chars.Length)]);
            return builder.ToString();
        }

        /// <summary>
        /// 生成流水号
        /// [日期 8位][时间 4位][自定义标识 n位][随机数 4位][Nonce 4位]
        /// </summary>
        /// <param name="customNum"></param>
        /// <returns></returns>
        private static readonly object _uuidLock = new object();
        private static int _uuidNonce = 0;
        private static string _lastTimeTag = DateTime.Now.ToString("HHmmss");
        public static string GenerateUUID(string customTag = null)
        {
            StringBuilder builder = new StringBuilder();
            DateTime nowTime = DateTime.Now;
            // 日期+时间
            builder.Append(nowTime.ToString("yyyyMMddHHmm"));
            Random random = new Random(nowTime.Second * 1000 + nowTime.Millisecond);
            // 自定义标识
            if (customTag != null) builder.Append(customTag);
            // 随机数
            builder.Append(random.Next(10000).ToString().PadLeft(4, '0'));
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
            return builder.ToString();
        }
    }
}
