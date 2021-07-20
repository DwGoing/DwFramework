using System;
using System.Collections.Generic;

namespace DwFramework.Core.Cache
{
    public interface ICache
    {
        // 对象操作
        public void Set(string key, object value);
        public void Set(string key, object value, DateTime expireAt);
        public void Set(string key, object value, TimeSpan expireTime);
        public object Get(string key);
        public T Get<T>(string key) where T : class;
        public void Del(string key);
        public string[] AllKeys();
        public string[] KeysWhere(string pattern);
        // Hash操作
        public void HSet(string key, string field, object value);
        public T HGet<T>(string key, string field);
        public Dictionary<string, object> HGetAll(string key);
        public void HDel(string key, string field);
        // 生命周期
        public void SetExpireTime(string key, DateTime expireAt);
        public void SetExpireTime(string key, TimeSpan expireTime);
    }
}