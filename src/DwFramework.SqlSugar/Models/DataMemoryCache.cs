using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;

namespace DwFramework.SqlSugar
{
    public class DataMemoryCache : ICacheService
    {
        private readonly MemoryCache _cacheManager;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataMemoryCache()
        {
            _cacheManager = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add<T>(string key, T value)
        {
            _cacheManager.Set(key, value);
        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireSeconds"></param>
        public void Add<T>(string key, T value, int expireSeconds)
        {
            _cacheManager.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSeconds),
            });
        }

        /// <summary>
        /// key是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey<T>(string key)
        {
            return _cacheManager.Get(key) != null;
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return _cacheManager.Get<T>(key);
        }

        /// <summary>
        /// 获取所有key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<string> GetAllKey<T>()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = typeof(MemoryCache).GetField("_entries", flags).GetValue(_cacheManager);
            var keys = new List<string>();
            if (entries is not IDictionary cacheItems) return keys;
            foreach (var item in cacheItems) keys.Add(((DictionaryEntry)item).Key.ToString());
            return keys;
        }

        /// <summary>
        /// 获取缓存值,若不存在则创建一个默认缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="create"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        public T GetOrCreate<T>(string key, Func<T> create, int expireSeconds = 30)
        {
            return _cacheManager.GetOrCreate<T>(key, cache =>
            {
                // 防止高并发时的穿透
                cache.SetAbsoluteExpiration(TimeSpan.FromSeconds(expireSeconds));
                cache.SetPriority(CacheItemPriority.Normal);
                return default;
            });
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public void Remove<T>(string key)
        {
            if (ContainsKey<T>(key)) _cacheManager.Remove(key);
        }
    }
}