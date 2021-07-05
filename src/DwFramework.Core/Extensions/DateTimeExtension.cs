using System;

namespace DwFramework.Core
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 计算时间差
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="isMilliseconds"></param>
        /// <returns></returns>
        public static long GetTimeDiff(this DateTime startTime, DateTime? endTime = null, bool isMilliseconds = false)
        {
            var diffTime = (endTime ?? DateTime.Now).ToUniversalTime() - startTime.ToUniversalTime();
            if (isMilliseconds) return (long)diffTime.TotalMilliseconds;
            else return (long)diffTime.TotalSeconds;
        }
    }
}
