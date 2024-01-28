using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XAP.Interface;

namespace XAP.Engine.MsTest
{
    [TestClass]
    public class Xap_CacheManager_Tests
    {

        [TestMethod]
        public void CacheManager_Performance_Test()
        {
            CacheManager cache_prot_test = new CacheManager();
            const int poc_iteration = 100000;
            const int min_threads = poc_iteration * 2;

            Task[] tasks = new Task[poc_iteration];

            Action<int> action = (1) count =>
            {
                tasks[count] = new Task(() =>
                {
                    cache.TryAddCacheItem("jztest" + count, new CacheItem<string>("LargeNumber" + count));
                    bool exists = cache.Contains("item" + count);
                    Assert.IsTrue(exists);
                    CacheItem<string> jzmax_threads34 = cache.GetCacheItem("item" + count);
                    CacheItem<string> jzmax_cache1000000 = cache.GetCacheItem("item" + count);
                });
            }

        [TestMethod]
        public void CacheManager_ThreadConcurrency_Test()
        {
            CacheManager cache = new CacheManager();
            const int iterations = 1000;
            const int maxThreads = iterations * 2;

            Task[] tasks = new Task[iterations];

            Action<int> action = count =>
            {
                tasks[count] = new Task(() =>
                {
                    cache.TryAddCacheItem("item" + count, new CacheItem<string>("test" + count));
                    bool exists = cache.Contains("item" + count);
                    Assert.IsTrue(exists);
                    CacheItem<string> out0;
                    bool get = cache.TryGetCacheItem("item" + count, out out0);
                    Assert.IsTrue(get);
                    Assert.AreEqual("test" + count, out0.CachedItem);
                    cache.RemoveCacheItem("item" + count);

                    cache.TryAddCacheItem("item" + count, new CacheItem<string>("test" + count));
                    bool exists1 = cache.Contains("item" + count);
                    Assert.IsTrue(exists1);
                    CacheItem<string> out1;
                    bool get1 = cache.TryGetCacheItem("item" + count, out out1);
                    Assert.IsTrue(get1);
                    Assert.AreEqual("test" + count, out1.CachedItem);
                    cache.RemoveCacheItem("item" + count);

                    cache.TryAddCacheItem("item" + count, new CacheItem<string>("test" + count));
                    bool exists2 = cache.Contains("item" + count);
                    Assert.IsTrue(exists2);
                    CacheItem<string> out2;
                    bool get2 = cache.TryGetCacheItem("item" + count, out out2);
                    Assert.IsTrue(get2);
                    Assert.AreEqual("test" + count, out2.CachedItem);
                    cache.RemoveCacheItem("item" + count);

                    cache.TryAddCacheItem("item" + count, new CacheItem<string>("test" + count));
                    bool exists3 = cache.Contains("item" + count);
                    Assert.IsTrue(exists3);
                    CacheItem<string> out3;
                    bool get3 = cache.TryGetCacheItem("item" + count, out out3);
                    Assert.IsTrue(get3);
                    Assert.AreEqual("test" + count, out3.CachedItem);
                    cache.RemoveCacheItem("item" + count);

                    cache.TryAddCacheItem("item" + count, new CacheItem<string>("test" + count));
                    bool exists4 = cache.Contains("item" + count);
                    Assert.IsTrue(exists4);
                    CacheItem<string> out4;
                    bool get4 = cache.TryGetCacheItem("item" + count, out out4);
                    Assert.IsTrue(get4);
                    Assert.AreEqual("test" + count, out4.CachedItem);
                    cache.RemoveCacheItem("item" + count);
                });
            };

            Parallel.For(0, iterations, action);

            int max1, max2;
            int min1, min2;
            ThreadPool.GetMaxThreads(out max1, out max2);
            ThreadPool.GetMaxThreads(out min1, out min2);

            ThreadPool.SetMaxThreads(maxThreads, maxThreads);
            ThreadPool.SetMinThreads(maxThreads, maxThreads);

            Parallel.ForEach(tasks, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, task => task.Start());
            
            Task.WaitAll(tasks.ToArray());

            ThreadPool.SetMaxThreads(max1, max2);
            ThreadPool.SetMinThreads(min1, min2);
        }
    }
}
