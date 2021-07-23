using System;
using System.Threading;

namespace DwFramework.Core.Generator
{
    /// <summary>
    /// 雪花算法
    /// </summary>
    public sealed class SnowflakeGenerator
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
            public SnowflakeIdInfo(long id, DateTime startTime)
            {
                ID = id;
                StartTime = startTime;
                var timestamp = id >> (WorkerIdBits + SequenceBits);
                Timestamp = timestamp + DateTime.UnixEpoch.GetTimeDiff(startTime);
                WorkId = (id ^ (timestamp << (WorkerIdBits + SequenceBits))) >> SequenceBits;
                Sequence = id ^ (timestamp << (WorkerIdBits + SequenceBits) | WorkId << SequenceBits);
            }
        }

        private readonly object _lock = new object();
        private long _currentTimestamp;
        private long _currentSequence;

        public static int TimestampBits { get; } = 41;
        public static int WorkerIdBits { get; } = 10;
        public static int SequenceBits { get; } = 12;
        public long WorkerId { get; }
        public DateTime StartTime { get; }
        public long MaxTimestamp { get; }
        public long MaxWorkerId { get; }
        public long MaxSequence { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workerId"></param>
        /// <param name="startTime"></param>
        public SnowflakeGenerator(long workerId, DateTime startTime)
        {
            MaxTimestamp = 2L << TimestampBits;
            MaxWorkerId = (2L << WorkerIdBits) - 1;
            MaxSequence = (2L << SequenceBits) - 1;
            if (workerId < 0 || workerId > MaxWorkerId) throw new Exception("机器ID超过上限");
            WorkerId = workerId;
            StartTime = startTime;
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
        public static SnowflakeIdInfo DecodeId(long id, DateTime startTime) => new SnowflakeIdInfo(id, startTime);
    }
}
