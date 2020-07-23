using System;

namespace DwFramework.Core.Plugins
{
    public class MemoryCache : ICache
    {
        private MemoryCacheStore[] _memoryCacheStores;

        public MemoryCache(int storeCount = 6)
        {
            _memoryCacheStores = new MemoryCacheStore[storeCount];
            for (int i = 0; i < _memoryCacheStores.Length; i++)
                _memoryCacheStores[i] = new MemoryCacheStore();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value, DateTime expireAt)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value, TimeSpan expireTime)
        {
            throw new NotImplementedException();
        }
    }
}
