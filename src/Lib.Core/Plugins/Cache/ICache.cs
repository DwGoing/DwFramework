using System;

namespace DwFramework.Core.Plugins
{
    public interface ICache
    {
        public void Set(string key, object value);
        public void Set(string key, object value, DateTime expireAt);
        public void Set(string key, object value, TimeSpan expireTime);
        public void HSet(string key, string field, object value);
        public T Get<T>(string key);
        public T HGet<T>(string key, string field);
        public void Del(string key);
        public void HDel(string key, string field);
        public void SetExpireTime(string key, DateTime expireAt);
        public void SetExpireTime(string key, TimeSpan expireTime);
    }
}