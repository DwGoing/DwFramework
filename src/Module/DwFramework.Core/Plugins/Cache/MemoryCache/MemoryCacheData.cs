using System;

namespace DwFramework.Core.Plugins
{
    public sealed class MemoryCacheData
    {
        public readonly string Key;
        public readonly object Value;
        public DateTime? ExpireAt { get; private set; } = null;
        public bool IsExpired
        {
            get
            {
                if (ExpireAt == null) return false;
                return ExpireAt <= DateTime.UtcNow;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public MemoryCacheData(string key, object value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="expireAt"></param>
        public void SetExpireTime(DateTime expireAt)
        {
            ExpireAt = expireAt.ToUniversalTime();
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="expireTime"></param>
        public void SetExpireTime(TimeSpan expireTime)
        {
            var expireAt = DateTime.UtcNow.Add(expireTime);
            SetExpireTime(expireAt);
        }
    }
}
