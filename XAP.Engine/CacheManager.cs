using System;
using System.Collections.Concurrent;
using System.Threading;
using XAP.Interface;

namespace XAP.Engine
{
    public class CacheManager : ICacheManager, IDisposable
    {
        private readonly ConcurrentDictionary<string, ICacheItem> cache;
        private readonly Timer freshnessTimer;
        private bool isDisposed;

        public CacheManager()
        {
            cache = new ConcurrentDictionary<string, ICacheItem>();
            freshnessTimer = new Timer(FreshnessTimerCallback, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private void FreshnessTimerCallback(object state)
        {
            DateTime now = DateTime.UtcNow;

            foreach (var item in cache)
            {
               
                if (!item.Value.ValidUntil.HasValue)
                {
                    continue;
                }

                if (item.Value.ValidUntil < now)
                {
                    RemoveCacheItem(item.Key);
                }
            }
        }

        public bool TryAddCacheItem<T>(string key, CacheItem<T> cacheItem) 
        {
            return cache.TryAdd(key, cacheItem);
        }

        public bool TryGetCacheItem<T>(string key, out CacheItem<T> item)
        {
            ICacheItem returnObject;

            bool exists = cache.TryGetValue(key, out returnObject);

            item = returnObject as CacheItem<T>;

            return exists;
        }

        public void RemoveCacheItem(string key)
        {
            ICacheItem ignore;
            cache.TryRemove(key, out ignore);
        }

        public bool Contains(string key)
        {
            return cache.ContainsKey(key);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing && freshnessTimer != null)
            {
                freshnessTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                freshnessTimer.Dispose();
            }
            
            isDisposed = true;
        }
    }
}
