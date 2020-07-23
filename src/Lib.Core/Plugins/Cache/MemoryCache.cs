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
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return GetMemoryCacheStore(key).Get<T>(key);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            GetMemoryCacheStore(key).Set(key, value);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireAt"></param>
        public void Set(string key, object value, DateTime expireAt)
        {
            GetMemoryCacheStore(key).Set(key, value, expireAt);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        public void Set(string key, object value, TimeSpan expireTime)
        {
            GetMemoryCacheStore(key).Set(key, value, expireTime);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            GetMemoryCacheStore(key).Remove(key);
        }
    }
}
