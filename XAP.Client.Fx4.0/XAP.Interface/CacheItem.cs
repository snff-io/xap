using System;

namespace XAP.Interface
{
    public class CacheItem<T> : ICacheItem
    {
        public DateTime? ValidUntil { get; private set; }

        public T CachedItem { get; private set; }

        public CacheItem(T item, DateTime? validUntil = default(DateTime?))
        {
            CachedItem = item;
            ValidUntil = validUntil;
        }
    }
}
