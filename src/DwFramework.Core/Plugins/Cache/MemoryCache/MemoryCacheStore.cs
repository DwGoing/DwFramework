using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;

namespace DwFramework.Core.Cache
{
    public sealed class MemoryCacheStore
    {
        private readonly Hashtable _datas;
        private readonly Timer _timer;
        private bool _isCleaning = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MemoryCacheStore()
        {
            _datas = Hashtable.Synchronized(new Hashtable());
            _timer = new Timer(30 * 1000)
            {
                AutoReset = true
            };
            _timer.Elapsed += CleanExpireData;
            _timer.Start();
        }

        /// <summary>
        /// 清理过期数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CleanExpireData(object sender, EventArgs args)
        {
            if (_isCleaning) return;
            var currentTime = DateTime.UtcNow;
            _isCleaning = true;
            var keys = _datas.Keys;
            keys.ForEach(item =>
            {
                var data = (MemoryCacheData)_datas[item];
                if (data.IsExpired) Del((string)item);
            });
            _isCleaning = false;
        }

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            var data = new MemoryCacheData(key, value);
            _datas[data.Key] = data;
        }

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireAt"></param>
        public void Set(string key, object value, DateTime expireAt)
        {
            var data = new MemoryCacheData(key, value);
            data.SetExpireTime(expireAt);
            _datas[data.Key] = data;
        }

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        public void Set(string key, object value, TimeSpan expireTime)
        {
            var data = new MemoryCacheData(key, value);
            data.SetExpireTime(expireTime);
            _datas[data.Key] = data;
        }

        /// <summary>
        /// 获取数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            var data = (MemoryCacheData)_datas[key];
            if (data == null) return null;
            if (data.IsExpired)
            {
                Del(key);
                return null;
            }
            return data.Value;
        }

        /// <summary>
        /// 获取数据（对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {
            var value = Get(key);
            if (value == null) return default;
            return value as T;
        }

        /// <summary>
        /// 删除数据（对象）
        /// </summary>
        /// <param name="key"></param>
        public void Del(string key)
        {
            _datas.Remove(key);
        }

        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <returns></returns>
        public string[] AllKeys()
        {
            var keys = _datas.Keys;
            var result = new string[keys.Count];
            keys.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// 正则获取Key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string[] KeysWhere(string pattern)
        {
            var keys = _datas.Keys;
            var result = new string[keys.Count];
            keys.CopyTo(result, 0);
            return result.Where(item => Regex.IsMatch(item, pattern)).ToArray();
        }

        /// <summary>
        /// 添加数据（Hash）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void HSet(string key, string field, object value)
        {
            var data = (MemoryCacheData)_datas[key];
            if (data == null) data = new MemoryCacheData(key, Hashtable.Synchronized(new Hashtable()));
            (data.Value as Hashtable).Add(field, value);
            _datas[data.Key] = data;
        }

        /// <summary>
        /// 获取数据（Hash）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HGet<T>(string key, string field)
        {
            var table = Get<Hashtable>(key);
            if (table == null) return default;
            if (!table.ContainsKey(field) || table[field].GetType() is T) return default;
            return (T)table[field];
        }

        /// <summary>
        /// 获取所有数据（Hash）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, object> HGetAll(string key)
        {
            var table = Get<Hashtable>(key);
            if (table == null) return default;
            var dic = new Dictionary<string, object>();
            table.ForEach(item => dic.Add((string)((DictionaryEntry)item).Key, ((DictionaryEntry)item).Value));
            return dic;
        }

        /// <summary>
        /// 删除数据（Hash）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        public void HDel(string key, string field)
        {
            var table = Get<Hashtable>(key);
            if (table == null) return;
            table.Remove(field);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt"></param>
        public void SetExpireTime(string key, DateTime expireAt)
        {
            var data = (MemoryCacheData)_datas[key];
            if (data == null) return;
            data.SetExpireTime(expireAt);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireTime"></param>
        public void SetExpireTime(string key, TimeSpan expireTime)
        {
            var data = (MemoryCacheData)_datas[key];
            if (data == null) return;
            data.SetExpireTime(expireTime);
        }
    }
}
