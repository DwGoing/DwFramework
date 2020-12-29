using System;
using System.Text;
using System.Threading;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    /// <summary>
    /// 雪花算法
    /// </summary>
    public sealed class SnowflakeGenerater
    {
        public sealed class SnowflakeIdInfo
        {
            public long ID { get; }
            public DateTime StartTime { get; }
            public long Timestamp { get; }
            public DateTime Time => DateTime.UnixEpoch.AddSeconds(Timestamp);
            public long WorkId { get; }
            public long Sequence { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="id"></param>
            /// <param name="startTime"></param>
            public SnowflakeIdInfo(long id, DateTime? startTime = null)
            {
                ID = id;
                StartTime = startTime != null ? startTime.Value : DateTime.UnixEpoch;
                var timestamp = id >> (SnowflakeGenerater.WorkerIdBits + SnowflakeGenerater.SequenceBits);
                Timestamp = timestamp + DateTime.UnixEpoch.GetTimeDiff(startTime);
                WorkId = (id ^ (timestamp << (SnowflakeGenerater.WorkerIdBits + SnowflakeGenerater.SequenceBits))) >> SnowflakeGenerater.SequenceBits;
                Sequence = id ^ (timestamp << (SnowflakeGenerater.WorkerIdBits + SnowflakeGenerater.SequenceBits) | WorkId << SnowflakeGenerater.SequenceBits);
            }
        }

        public static readonly int TimestampBits = 41;
        public static readonly int WorkerIdBits = 10;
        public static readonly int SequenceBits = 12;

        private readonly object _lock = new object();
        private long _currentTimestamp;
        private long _currentSequence;

        public readonly long WorkerId;
        public readonly DateTime StartTime;
        public readonly long MaxTimestamp;
        public readonly long MaxWorkerId;
        public readonly long MaxSequence;

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
            StartTime = startTime != null ? startTime.Value : DateTime.UnixEpoch;
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
                var timestamp = StartTime.GetTimeDiff(DateTime.Now);
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

        /// <summary>
        /// 解析ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static SnowflakeIdInfo DecodeId(long id, DateTime? startTime = null) => new SnowflakeIdInfo(id, startTime);
    }

    /// <summary>
    /// UUID生成器
    /// </summary>
    public static class UUIDGenerater
    {
        private static readonly object _lock = new object();
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
                var timestamp = DateTime.UnixEpoch.GetTimeDiff(DateTime.UtcNow);
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
