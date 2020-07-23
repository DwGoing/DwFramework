using System;

namespace DwFramework.Core.Plugins
{
    public interface ICache
    {
        public T Get<T>(string key);
        public void Set(string key, object value);
        public void Set(string key, object value, DateTime expireAt);
        public void Set(string key, object value, TimeSpan expireTime);
        public void Remove(string key);
    }
}