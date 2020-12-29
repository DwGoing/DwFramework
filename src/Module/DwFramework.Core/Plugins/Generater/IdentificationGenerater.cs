using System;
using System.Text;
using System.Threading;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    /// <summary>
    /// 雪花算法
    /// </summary>
    public class SnowflakeGenerater
    {
        private readonly object _lock = new object();

        public readonly long WorkerId;
        public readonly DateTime StartTime;
        public readonly int TimestampBits = 41;
        public readonly long MaxTimestamp;
        public readonly int WorkerIdBits = 10;
        public readonly long MaxWorkerId;
        public readonly int SequenceBits = 12;
        public readonly long MaxSequence;

        private long _currentTimestamp;
        private long _currentSequence;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="startTime"></param>
        public SnowflakeGenerater(long workerId = 0, DateTime? startTime = null)
        {
            MaxTimestamp = 2L << TimestampBits;
            MaxWorkerId = (2L << WorkerIdBits) - 1;
            MaxSequence = (2L << SequenceBits) - 1;

            if (workerId < 0 || workerId > MaxWorkerId) throw new Exception("机器ID超过上限");
            WorkerId = workerId;
            StartTime = startTime != null ? startTime.Value : DateTime.Parse("1970.01.01");
        }

        /// <summary>
        /// 雪花算法
        /// </summary>
        /// <returns></returns>
        public long GenerateId()
        {
            lock (_lock)
            {
                _currentSequence++;
                if (_currentSequence > MaxSequence) Thread.Sleep(1);

                var timestamp = StartTime.ToUniversalTime().GetTimeDiff(DateTime.UtcNow);
                if (timestamp > MaxTimestamp) throw new Exception("时间戳容量不足,请调整StartTime");
                if (timestamp < _currentTimestamp) throw new Exception("时间获取异常,请检查服务器时间");
                else if (timestamp > _currentTimestamp)
                {
                    _currentTimestamp = timestamp;
                    _currentSequence = 0;
                }

                return _currentTimestamp << (WorkerIdBits + SequenceBits) | WorkerId << SequenceBits | _currentSequence;
            }
        }
    }

    /// <summary>
    /// UUID生成器
    /// </summary>
    public static class UUIDGenerater
    {
        private static readonly object _lock = new object();

        private static readonly DateTime _startTime = DateTime.Parse("1970.01.01");
        private static int _nonce = 0;
        private static long _currentTimestamp = 0;

        /// <summary>
        /// UUID
        /// [自定义标识 n位][日期 8位][时间 4位][随机数 8位][Nonce 4位]
        /// </summary>
        /// <param name="customTag"></param>
        /// <returns></returns>
        public static string GenerateUUID(string customTag = null)
        {
            lock (_lock)
            {
                var builder = new StringBuilder();
                // 自定义标识
                if (customTag != null) builder.Append(customTag);
                var nowTime = DateTime.Now;
                // 日期+时间
                builder.Append(nowTime.ToString("yyyyMMddHHmm"));
                var random = RandomGenerater.GetRandom();
                // 随机数
                builder.Append(random.Next(100000000).ToString().PadLeft(8, '0'));
                // Nonce
                _nonce++;
                if (_nonce > 2L << 4) Thread.Sleep(1);
                var timestamp = _startTime.ToUniversalTime().GetTimeDiff(DateTime.UtcNow);
                if (timestamp < _currentTimestamp) throw new Exception("时间获取异常,请检查服务器时间");
                else if (timestamp > _currentTimestamp)
                {
                    _currentTimestamp = timestamp;
                    _nonce = 0;
                }
                builder.Append(_nonce.ToString().PadLeft(4, '0'));
                return builder.ToString();
            }
        }
    }
}
