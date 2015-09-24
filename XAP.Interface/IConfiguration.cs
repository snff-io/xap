using System.Collections.Generic;

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
        IEnumerable<ReporterConfiguration> GetReporterTypes();
        IEnumerable<Route> GetRouteManyRoutes();
        IEnumerable<Route> GetRouteOneRoutes();
        IEnumerable<ScheduledActionConfiguration> GetScheduledActions();
        string WorkerMode { get; set; }


        void SetAlertProperties(List<AlertProperty> properties);
        void SetAssemblyLoaderType(SimpleTypeDefinition type);
        void SetPersistenceManagerType(SimpleTypeDefinition type);
        void SetAlertQueueType(SimpleTypeDefinition type);
        void SetCheckpointManagerType(SimpleTypeDefinition type);
        void SetActionTypes(IEnumerable<SimpleTypeDefinition> actionTypes);
        void SetReporterTypes(IEnumerable<ReporterConfiguration> reporters);
        void SetRouteManyRoutes(IEnumerable<Route> routes);
        void SetRouteOneRoutes(IEnumerable<Route> routes);
        void SetScheduledActions(IEnumerable<ScheduledActionConfiguration> actions);

    }
}
