using System;

namespace DwFramework.Core.Helper
{
    public static class DateHelper
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="isMilliseconds"></param>
        /// <returns></returns>
        public static long GetTimestamp(bool isMilliseconds = false)
        {
            var diffTime = DateTime.UtcNow - DateTime.Parse("1970-01-01");
            if (isMilliseconds) return (long)diffTime.TotalMilliseconds;
            else return (long)diffTime.TotalSeconds;
        }
    }
}
