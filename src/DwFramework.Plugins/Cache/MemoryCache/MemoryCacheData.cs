using System;

namespace DwFramework.Core.Cache
{
    public sealed class MemoryCacheData
    {
        public string Key { get; init; }
        public object Value { get; init; }
        public DateTime? ExpireAt { get; private set; } = null;
        public bool IsExpired => ExpireAt != null || ExpireAt <= DateTime.Now;

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
            ExpireAt = expireAt;
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="expireTime"></param>
        public void SetExpireTime(TimeSpan expireTime)
        {
            var expireAt = DateTime.Now.Add(expireTime);
            SetExpireTime(expireAt);
        }
    }
}
