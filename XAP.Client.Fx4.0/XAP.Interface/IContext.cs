using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IContext
    {
        IAlertQueue AlertQueue { get; }

        IWorkerFactory Factory { get; }

        ICheckpointManager CheckpointManger { get; }

        IPersistenceManager Persistence { get; }

        IEnumerable<IReporter> Reporters { get; }

        IPerformanceManager Performance { get; }

        IConfiguration Configuration { get; }

        IRouteManager RouteManager { get; }

        ICacheManager CacheManager { get; }
    }
}
