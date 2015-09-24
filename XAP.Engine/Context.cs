using System.Collections.Generic;
using XAP.Interface;

namespace XAP.Engine
{
    public class Context:IContext
    {

        public IAlertQueue AlertQueue
        {
            get;
            set;
        }

        public ILockManager CheckpointManger
        {
            get;
            set;
        }

        public IEnumerable<IReporter> Reporters
        {
            get;
            set;
        }

        public IWorkerFactory Factory
        {
            get;
            set;
        }

        public IPerformanceManager Performance
        {
            get;
            set;
        }

        public IPersistenceManager Persistence
        {
            get;
            set;
        }

        public IConfiguration Configuration
        {
            get;
            set;
        }

        public IRouteManager RouteManager
        {
            get;
            set;
        }

        public ICacheManager CacheManager
        {
            get;
            set;
        }
    }
}
