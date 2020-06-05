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
        /// [日期 6位][时间 4位][随机数 6位][自定义号段 4位][盐 8位]
        /// </summary>
        /// <param name="customNum"></param>
        /// <returns></returns>
        private static object _uuidLock = new object();
        private static int _uuidNorce = 0;
        private static string _lastTimeTag = DateTime.Now.ToString("HHmmss");
        public static string GenerateUUID(string customNum = "0000")
        {
            StringBuilder builder = new StringBuilder();
            DateTime nowTime = DateTime.Now;
            // 日期+时间
            builder.Append(nowTime.ToString("yyyyMMddHHmm"));
            Random random = new Random(nowTime.Second * 1000 + nowTime.Millisecond);
            // 随机数
            builder.Append(random.Next(1000000).ToString().PadLeft(6, '0'));
            // 自定义号段
            if (customNum.Length != 4)
                throw new Exception("自定义号段长度错误");
            builder.Append(customNum);
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
            builder.Append(_uuidNorce.ToString().PadLeft(8, '0'));
            return builder.ToString();
        }
    }
}
