using System;

namespace DwFramework.Core.Plugins
{
    public class Stopwatch
    {
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startTime"></param>
        public Stopwatch(DateTime? startTime = null)
        {
            SetStartTime(startTime);
        }

        /// <summary>
        /// 设置开始时间
        /// </summary>
        /// <param name="startTime"></param>
        public void SetStartTime(DateTime? startTime = null)
        {
            StartTime = startTime.HasValue ? startTime.Value.ToUniversalTime() : DateTime.UtcNow;
        }

        /// <summary>
        /// 获取总毫秒
        /// </summary>
        /// <returns></returns>
        public long GetTotalMilliseconds()
        {
            return (long)(DateTime.UtcNow - StartTime).TotalMilliseconds;
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
            public static DateTime StartTime { get; private set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            static Static()
            {
                SetStartTime(DateTime.Now);
            }

            /// <summary>
            /// 设置开始时间
            /// </summary>
            /// <param name="startTime"></param>
            public static void SetStartTime(DateTime? startTime = null)
            {
                StartTime = startTime.HasValue ? startTime.Value.ToUniversalTime() : DateTime.UtcNow;
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
}
