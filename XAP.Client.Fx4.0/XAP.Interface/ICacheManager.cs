namespace XAP.Interface
{
    public interface ICacheManager
    {
        bool TryAddCacheItem<T>(string key, CacheItem<T> cacheItem);
        bool TryGetCacheItem<T>(string key, out CacheItem<T> item);
        void RemoveCacheItem(string key);
        bool Contains(string key);
    }
}
