using System;
using System.Text;
using System.Threading;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    public static class IdentificationGenerater
    {
        #region 雪花算法
        private static readonly object _snowflake_lock = new object();

        private static int _snowflake_timestampBits = 41;
        private static long _snowflake_maxTimestamp = 2L << _snowflake_timestampBits;
        private static int _snowflake_workerIdBits = 10;
        private static long _snowflake_maxWorkerId = (2L << _snowflake_workerIdBits) - 1;
        private static int _snowflake_sequenceBits = 12;
        private static long _snowflake_maxSequence = (2L << _snowflake_sequenceBits) - 1;
        private static long _snowflake_currentTimestamp = 0;
        private static long _snowflake_currentSequence = 0;

        public static DateTime StartTime = DateTime.Parse("1970.01.01");

        /// <summary>
        /// 雪花算法
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns></returns>
        public static long Snowflake(long workerId = 0)
        {
            lock (_snowflake_lock)
            {
                if (workerId < 0 || workerId > _snowflake_maxWorkerId) throw new Exception("机器ID超过上限");

                _snowflake_currentSequence++;
                if (_snowflake_currentSequence > _snowflake_maxSequence) Thread.Sleep(1);

                var timestamp = StartTime.ToUniversalTime().GetTimeDiff(DateTime.UtcNow);
                if (timestamp > _snowflake_maxTimestamp) throw new Exception("时间戳容量不足,请调整StartTime");
                if (timestamp < _snowflake_currentTimestamp) throw new Exception("时间获取异常,请检查服务器时间");
                else if (timestamp > _snowflake_currentTimestamp)
                {
                    _snowflake_currentTimestamp = timestamp;
                    _snowflake_currentSequence = 0;
                }

                var id = _snowflake_currentTimestamp << (_snowflake_workerIdBits + _snowflake_sequenceBits) | workerId << _snowflake_sequenceBits | _snowflake_currentSequence;
                return id;
            }
        }
        #endregion

        #region UUID
        private static readonly object _uuid_lock = new object();

        private static int _uuid_nonce = 0;
        private static long _uuid_currentTimestamp = 0;

        /// <summary>
        /// UUID
        /// [自定义标识 n位][日期 8位][时间 4位][随机数 8位][Nonce 4位]
        /// </summary>
        /// <param name="customTag"></param>
        /// <returns></returns>
        public static string UUID(string customTag = null)
        {
            lock (_uuid_lock)
            {
                StringBuilder builder = new StringBuilder();
                // 自定义标识
                if (customTag != null) builder.Append(customTag);
                DateTime nowTime = DateTime.Now;
                // 日期+时间
                builder.Append(nowTime.ToString("yyyyMMddHHmm"));
                Random random = new Random((int)RandomGenerater.RandomNumber());
                // 随机数
                builder.Append(random.Next(100000000).ToString().PadLeft(8, '0'));
                // Nonce
                _uuid_nonce++;
                if (_uuid_nonce > 2L << 4) Thread.Sleep(1);
                var timestamp = StartTime.ToUniversalTime().GetTimeDiff(DateTime.UtcNow);
                if (timestamp < _uuid_currentTimestamp) throw new Exception("时间获取异常,请检查服务器时间");
                else if (timestamp > _uuid_currentTimestamp)
                {
                    _uuid_currentTimestamp = timestamp;
                    _uuid_nonce = 0;
                }
                builder.Append(_uuid_nonce.ToString().PadLeft(4, '0'));
                return builder.ToString();
            }
        }
        #endregion
    }
}
