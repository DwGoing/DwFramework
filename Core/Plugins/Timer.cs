using System;

namespace DwFramework.Core.Plugins
{
    public class Timer
    {
        public DateTime? StartTime { get; private set; } = null;

        /// <summary>
        /// 设置开始时间
        /// </summary>
        /// <param name="startTime"></param>
        public void SetStartTime(DateTime? startTime = null)
        {
            StartTime = startTime ?? DateTime.UtcNow;
        }

        /// <summary>
        /// 获取总毫秒
        /// </summary>
        /// <returns></returns>
        public long GetTotalMilliseconds()
        {
            if (!StartTime.HasValue) throw new Exception("未设置开始时间");
            return (long)(DateTime.UtcNow - StartTime.Value.ToUniversalTime()).TotalMilliseconds;
        }

        /// <summary>
        /// 获取总秒
        /// </summary>
        /// <returns></returns>
        public long GetTotalSeconds()
        {
            return GetTotalMilliseconds() / 1000;
        }

        public static class Static
        {
            public static DateTime? StartTime { get; private set; } = null;

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
                if (!StartTime.HasValue) throw new Exception("未设置开始时间");
                return (long)(DateTime.UtcNow - StartTime.Value.ToUniversalTime()).TotalMilliseconds;
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
}
