using System;

namespace DwFramework.Core.Filter
{
    public sealed class BloomFilter
    {
        private readonly object _lock = new object();
        private readonly int[] _countArray;
        private readonly int _countOfHash;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="size"></param>
        /// <param name="countOfHash"></param>
        public BloomFilter(long size = 100000, int countOfHash = 3)
        {
            _countArray = new int[size];
            _countOfHash = countOfHash;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="value"></param>
        public void Add(object value)
        {
            lock (_lock)
            {
                var random = new Random(value.GetHashCode());
                for (var i = 0; i < _countOfHash; i++)
                {
                    _countArray[random.Next(_countArray.Length - 1)]++;
                }
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="value"></param>
        public void Remove(object value)
        {
            lock (_lock)
            {
                var random = new Random(value.GetHashCode());
                for (var i = 0; i < _countOfHash; i++)
                {
                    _countArray[random.Next(_countArray.Length - 1)]--;
                }
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsExist(object value)
        {
            lock (_lock)
            {
                var random = new Random(value.GetHashCode());
                for (var i = 0; i < _countOfHash; i++)
                {
                    if (_countArray[random.Next(_countArray.Length - 1)] == 0)
                        return false;
                }
                return true;
            }
        }
    }
}
