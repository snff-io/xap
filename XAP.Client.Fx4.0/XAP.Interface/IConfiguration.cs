using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface IConfiguration
    {
        List<AlertProperty> GetAlertProperties();
        SimpleTypeDefinition GetAssemblyLoaderType();
        SimpleTypeDefinition GetPersistenceManagerType();
        SimpleTypeDefinition GetAlertQueueType();
        SimpleTypeDefinition GetCheckpointManagerType();
        IEnumerable<SimpleTypeDefinition> GetActionTypes();
        IEnumerable<SimpleTypeDefinition> GetReporterTypes();
        IEnumerable<Route> GetRouteManyRoutes();
        IEnumerable<Route> GetRouteOneRoutes();
        IEnumerable<ScheduledActionConfiguration> GetScheduledActions();
        string WorkerMode { get; }
    }
}
