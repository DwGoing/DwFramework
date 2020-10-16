using System;

namespace DwFramework.Core.Plugins
{
    public class BloomFilter
    {
        private readonly int[] _countArray;
        private readonly static object _countArrayLock = new object();
        private readonly int _numberOfHash;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="size"></param>
        public BloomFilter(long size = 100000, int numberOfHash = 3)
        {
            _countArray = new int[size];
            _numberOfHash = numberOfHash;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="value"></param>
        public void Add(object value)
        {
            var random = new Random(value.GetHashCode());
            for (var i = 0; i < _numberOfHash; i++)
            {
                _countArray[random.Next(_countArray.Length - 1)]++;
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value)
        {
            var random = new Random(value.GetHashCode());
            for (var i = 0; i < _numberOfHash; i++)
            {
                _countArray[random.Next(_countArray.Length - 1)]--;
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsExist(object value)
        {
            var random = new Random(value.GetHashCode());
            for (var i = 0; i < _numberOfHash; i++)
            {
                if (_countArray[random.Next(_countArray.Length - 1)] == 0)
                    return false;
            }
            return true;
        }
    }
}
