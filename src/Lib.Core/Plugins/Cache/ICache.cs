using System;

namespace DwFramework.Core.Plugins
{
    public interface ICache
    {
        // 对象操作
        public void Set(string key, object value);
        public void Set(string key, object value, DateTime expireAt);
        public void Set(string key, object value, TimeSpan expireTime);
        public T Get<T>(string key);
        public void Del(string key);
        // Hash操作
        public void HSet(string key, string field, object value);
        public T HGet<T>(string key, string field);
        public void HDel(string key, string field);
        // 生命周期
        public void SetExpireTime(string key, DateTime expireAt);
        public void SetExpireTime(string key, TimeSpan expireTime);
    }
}