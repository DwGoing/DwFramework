using System;

namespace DwFramework.Core.Plugins
{
    public static class Timer
    {
        public static DateTime StartTime { get; private set; }

        /// <summary>
        /// 设置开始时间
        /// </summary>
        /// <param name="startTime"></param>
        public static void SetStartTime(DateTime? startTime = null)
        {
            StartTime = startTime ?? DateTime.UtcNow;
        }

        /// <summary>
        /// 获取总毫秒
        /// </summary>
        /// <returns></returns>
        public static long GetTotalMilliseconds()
        {
            return (long)(DateTime.UtcNow - StartTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取总秒
        /// </summary>
        /// <returns></returns>
        public static long GetTotalSeconds()
        {
            return GetTotalMilliseconds() / 1000;
        }
    }
}
