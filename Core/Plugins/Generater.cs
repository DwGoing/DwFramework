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
        public static Guid GenerateGUID()
        {
            return Guid.NewGuid();
        }

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
            for (int i = 0; i < length; i++)
            {
                builder.Append(chars[random.Next(chars.Length)]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 生成流水号
        /// [日期 6位][时间 4位][随机数 6位][自定义号段1 4位][自定义号段2 4位][盐 4位]
        /// </summary>
        /// <param name="customNum1"></param>
        /// <param name="customNum2"></param>
        /// <returns></returns>
        private static object _uuidLock = new object();
        private static int _uuidNorce = 0;
        private static string _lastTimeTag = DateTime.Now.ToString("HHmmss");
        public static string GenerateUUID(string customNum1 = "0000", string customNum2 = "0000")
        {
            StringBuilder builder = new StringBuilder();
            DateTime nowTime = DateTime.Now;
            // 日期+时间
            builder.Append(nowTime.ToString("yyyyMMddHHmm"));
            Random random = new Random(nowTime.Second * 1000 + nowTime.Millisecond);
            // 随机数
            var value = random.Next(1000000).ToString();
            for (int i = 0; i < 6 - value.Length; i++)
            {
                builder.Append('0');
            }
            builder.Append(value);
            // 自定义号段
            if (customNum1.Length != 4 || customNum2.Length != 4)
                throw new Exception("自定义号段长度错误");
            builder.Append(customNum1);
            builder.Append(customNum2);
            lock (_uuidLock)
            {
                if (_lastTimeTag == nowTime.ToString("HHmmss"))
                {
                    _uuidNorce++;
                }
                else
                {
                    _lastTimeTag = nowTime.ToString("HHmmss");
                    _uuidNorce = 0;
                }
            }
            builder.Append(_uuidNorce);
            return builder.ToString();
        }
    }
}
