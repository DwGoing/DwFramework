using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DwFramework.Core.Plugins;

namespace DwFramework.Plugins.Core
{
    public class MemoryCacheStore
    {
        private readonly Hashtable _Datas;
        private readonly System.Timers.Timer _Timer;
        private bool IsClean = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MemoryCacheStore()
        {
            _Datas = Hashtable.Synchronized(new Hashtable());
            _Timer = new System.Timers.Timer(30 * 1000)
            {
                AutoReset = true
            };
            _Timer.Elapsed += CleanExpireData;
            _Timer.Start();
        }

        /// <summary>
        /// 清理过期数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CleanExpireData(object sender, EventArgs args)
        {
            if (IsClean) return;
            var currentTime = DateTime.UtcNow;
            TaskManager.CreateTask(() =>
            {
                IsClean = true;
                var keys = _Datas.Keys;
                foreach (var key in keys)
                {
                    var data = (MemoryCacheData)_Datas[key];
                    if (data.IsExpired) Del((string)key);
                }
                IsClean = false;
            });
        }

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="data"></param>
        private void Set(MemoryCacheData data) => _Datas[data.Key] = data;

        /// <summary>
        /// 获取数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private MemoryCacheData Get(string key) => (MemoryCacheData)_Datas[key];

        /// <summary>
        /// 添加数据（对象）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            Set(new MemoryCacheData(key, value));
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
            Set(data);
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
            Set(data);
        }

        /// <summary>
        /// 获取数据（对象）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            var data = Get(key);
            if (data == null) return default;
            if (data.IsExpired)
            {
                Del(key);
                return default;
            }
            if (data.Value.GetType() is T) return default;
            return (T)data.Value;
        }

        /// <summary>
        /// 删除数据（对象）
        /// </summary>
        /// <param name="key"></param>
        public void Del(string key) => _Datas.Remove(key);

        /// <summary>
        /// 获取所有Key
        /// </summary>
        /// <returns></returns>
        public string[] AllKeys()
        {
            var keys = _Datas.Keys;
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
            var keys = _Datas.Keys;
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
            MemoryCacheData data = Get(key);
            if (data == null) data = new MemoryCacheData(key, Hashtable.Synchronized(new Hashtable()));
            (data.Value as Hashtable).Add(field, value);
            Set(data);
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
            foreach (DictionaryEntry item in table)
            {
                dic.Add((string)item.Key, item.Value);
            }
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
            var data = Get(key);
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
            var data = Get(key);
            if (data == null) return;
            data.SetExpireTime(expireTime);
        }
    }
}
