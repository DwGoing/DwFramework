using System;

namespace DwFramework.Core.Plugins
{
    public class MemoryCache : ICache
    {
        private readonly MemoryCacheStore[] _memoryCacheStores;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="storeCount"></param>
        public MemoryCache(int storeCount = 6)
        {
            _memoryCacheStores = new MemoryCacheStore[storeCount];
            for (int i = 0; i < _memoryCacheStores.Length; i++)
                _memoryCacheStores[i] = new MemoryCacheStore();
        }

        /// <summary>
        /// 获取MemoryCacheStore
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private MemoryCacheStore GetMemoryCacheStore(string key)
        {
            var index = key.GetHashCode() & int.MaxValue % _memoryCacheStores.Length;
            return _memoryCacheStores[index];
        }

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            GetMemoryCacheStore(key).Set(key, value);
        }

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireAt"></param>
        public void Set(string key, object value, DateTime expireAt)
        {
            GetMemoryCacheStore(key).Set(key, value, expireAt);
        }

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        public void Set(string key, object value, TimeSpan expireTime)
        {
            GetMemoryCacheStore(key).Set(key, value, expireTime);
        }

        public void HSet(string key, string field, object value)
        {
            GetMemoryCacheStore(key).HSet(key, field, value);
        }

        /// <summary>
        /// 获取数据（对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return GetMemoryCacheStore(key).Get<T>(key);
        }

        public T HGet<T>(string key, string field)
        {
            return GetMemoryCacheStore(key).HGet<T>(key, field);
        }

        /// <summary>
        /// 删除数据（对象）
        /// </summary>
        /// <param name="key"></param>
        public void Del(string key)
        {
            GetMemoryCacheStore(key).Del(key);
        }

        /// <summary>
        /// 删除数据（Hash）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        public void HDel(string key, string field)
        {
            GetMemoryCacheStore(key).HDel(key, field);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt"></param>
        public void SetExpireTime(string key, DateTime expireAt)
        {
            GetMemoryCacheStore(key).SetExpireTime(key, expireAt);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireTime"></param>
        public void SetExpireTime(string key, TimeSpan expireTime)
        {
            GetMemoryCacheStore(key).SetExpireTime(key, expireTime);

        }
    }
}
