using System;
using System.Collections.Generic;

namespace DwFramework.Core.Plugins
{
    public class MemoryCacheStore
    {
        private readonly Dictionary<string, MemoryCacheData> _Datas;
        private readonly System.Timers.Timer _Timer;
        private bool IsClean = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MemoryCacheStore()
        {
            _Datas = new Dictionary<string, MemoryCacheData>();
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
                    if (_Datas[key].IsExpired)
                        _Datas.Remove(key);
                }
                IsClean = false;
            });
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (!_Datas.ContainsKey(key)) return default;
            var data = _Datas[key];
            if (_Datas[key].IsExpired)
            {
                _Datas.Remove(key);
                return default;
            }
            return (T)data.Value;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="data"></param>
        public void Set(MemoryCacheData data)
        {
            _Datas[data.Key] = data;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            Set(new MemoryCacheData(key, value));
        }

        /// <summary>
        /// 设置数据
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
        /// 设置数据
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
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (!_Datas.ContainsKey(key)) return;
            _Datas.Remove(key);
        }
    }
}
