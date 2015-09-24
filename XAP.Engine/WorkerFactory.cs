using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using XAP.Common;
using XAP.Engine.Reporter;
using XAP.Engine.Router;
using XAP.Interface;

namespace XAP.Engine
{
    public class WorkerFactory : Factory, IWorkerFactory
    {
        private readonly IConfiguration _config;
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly Dictionary<string, Type> _actionTypes;
        private readonly Dictionary<string, Type> _reporterTypes;

        public WorkerFactory(IConfiguration config)
        {
            try
            {
                Tracing.XapTrace.TraceInformation("constructing factory");
                _config = config;

                Tracing.XapTrace.TraceInformation("loading assembly loader");
                _assemblyLoader = LoadAssemblyLoaderInstance(_config.GetAssemblyLoaderType());

                Register<IRouteManager>(() => new RouteManager());
                Register<IRouterWorker>(() => new RouterWorker());
                Register<IReporterWorker>(() => new ReporterWorker());
                Register<IPerformanceManager>(() => new PerformanceManager());
                Register<IScheduledActionManager>(() => new ScheduledActionManager());
                Register<ICacheManager>(() => new CacheManager());
                Register<AlertInstance>(() => new AlertInstance());

                Tracing.XapTrace.TraceInformation("loading queue assembly");
                Register<IAlertQueue>(() => LoadInstanceFromAssembly<IAlertQueue>(_config.GetAlertQueueType()));

                Tracing.XapTrace.TraceInformation("loading checkpoint manager assembly");
                Register<ILockManager>(() => LoadInstanceFromAssembly<ILockManager>(_config.GetCheckpointManagerType()));

                Tracing.XapTrace.TraceInformation("loading persistence manager assembly");
                Register<IPersistenceManager>(() => LoadInstanceFromAssembly<IPersistenceManager>(_config.GetPersistenceManagerType()));

                Tracing.XapTrace.TraceInformation("loading action types");
                _actionTypes = LoadTypes(_config.GetActionTypes());

                Tracing.XapTrace.TraceInformation("loading reporter types");
                _reporterTypes = LoadTypes(_config.GetReporterTypes().Select(x => new SimpleTypeDefinition { AssemblyName = x.AssemblyName, TypeName = x.TypeName }));
            }
            catch (Exception ex)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Critical, 4, "factory initialization error.\r\n" + ex);
                throw;
            }
        }

        public IAlertAction CreateAlertActionInstance(string typeName, Dictionary<string, string> parameters)
        {
            Tracing.XapTrace.TraceInformation("create alertAction instance {0}", typeName);
            if (!_actionTypes.ContainsKey(typeName))
            {
                Tracing.XapTrace.TraceData(TraceEventType.Error, 0, typeName + " Is not a configured action");
                throw new Exception(typeName + " Is not a configured action");
            }

            var actionType = _actionTypes[typeName];
            var instance = actionType.InvokeMember("", BindingFlags.CreateInstance, null, null, null);
            var asAlertAction = instance as IAlertAction;

            if (asAlertAction == null)
            {
                var ae = new ApplicationException(string.Format("unable to create alertAction instance {0}", typeName));
                Tracing.XapTrace.TraceEvent(TraceEventType.Error, 13, ae.ToString());
                throw ae;
            }

            asAlertAction.SetParameters(parameters);

            return asAlertAction;
        }

        public IReporter CreateReporterInstance(string typeName)
        {
            Tracing.XapTrace.TraceInformation("create reporter instance {0}", typeName);

            var reporterType = _reporterTypes[typeName];
            var instance = reporterType.InvokeMember("", BindingFlags.CreateInstance, null, null, null);
            var asReporter = instance as IReporter;

            if (asReporter != null)
            {
                return asReporter;
            }

            var ae = new ApplicationException("create reporter instance " + typeName);
            Tracing.XapTrace.TraceEvent(TraceEventType.Error, 14, ae.ToString());
            throw ae;
        }

        public IEnumerable<IReporter> CreateAllReporters()
        {
            var reporters = new List<IReporter>();
            var reporterConfig = _config.GetReporterTypes();

            foreach (var config in reporterConfig)
            {
                var r = CreateReporterInstance(config.TypeName);
                r.SetParameters(config.Params);
                reporters.Add(r);
            }

            return reporters;
        }

        public T LoadInstanceFromAssembly<T>(SimpleTypeDefinition typeDef)
            where T : class
        {
            Tracing.XapTrace.TraceInformation("loading type from assembly:{0}!{1}", typeDef.AssemblyName, typeDef.TypeName);
            var type = _assemblyLoader.GetTypeFromAssembly(typeDef);

            Tracing.XapTrace.TraceInformation("creating instance of {0}", typeDef.TypeName);
            var instance = type.InvokeMember(typeDef.TypeName, BindingFlags.CreateInstance, null, null, null);
            var asT = instance as T;

            if (asT != null)
            {
                return asT;
            }

            var e = new ApplicationException(string.Format("could not load instance of {0}!{1}.", typeDef.AssemblyName, typeDef.TypeName));
            Tracing.XapTrace.TraceEvent(TraceEventType.Error, 12, e.ToString());
            throw e;
        }

        Dictionary<string, Type> LoadTypes(IEnumerable<SimpleTypeDefinition> typeDefs)
        {
            var ret = new Dictionary<string, Type>();

            foreach (var t in typeDefs)
            {
                Tracing.XapTrace.TraceInformation("loading type from assembly:{0}!{1}", t.AssemblyName, t.TypeName);
                var type = _assemblyLoader.GetTypeFromAssembly(t);

                ret[t.TypeName] = type;
            }

            return ret;
        }

        IAssemblyLoader LoadAssemblyLoaderInstance(SimpleTypeDefinition typeDef)
        {
            Tracing.XapTrace.TraceInformation("Assembly.Load type from assembly:{0}!{1}", typeDef.AssemblyName, typeDef.TypeName);
            var assembly = Assembly.Load(typeDef.AssemblyName);
            var type = assembly.GetType(typeDef.TypeName);

            Tracing.XapTrace.TraceInformation("create instance of {0}!{1}", typeDef.AssemblyName, typeDef.TypeName);
            var instance = type.InvokeMember("", BindingFlags.CreateInstance, null, null, null);
            var loader = instance as IAssemblyLoader;

            if (loader != null)
            {
                return loader;
            }

            var e = new ApplicationException(string.Format("could not load instance of {0}!{1}.", typeDef.AssemblyName, typeDef.TypeName));
            Tracing.XapTrace.TraceEvent(TraceEventType.Error, 12, e.ToString());
            throw e;
        }


        public AlertInstance CreateAlertInstance(string reporterName)
        {
            var alertInstance = CreateInstance<AlertInstance>();

            alertInstance.Properties = _config.GetAlertProperties();
            alertInstance.XapId = Guid.NewGuid();
            alertInstance.Reporter = reporterName;

            return alertInstance;
        }

        public void FixupAlertInstance(AlertInstance alertInstance)
        {
            if (alertInstance.XapId == Guid.Empty)
            {
                alertInstance.XapId = Guid.NewGuid();
            }

            var configuredProperties = _config.GetAlertProperties();

            var missingProperties = configuredProperties.Where(x => alertInstance.Properties.All(y => y.Name != x.Name));

            alertInstance.Properties.AddRange(missingProperties);
        }

        public void ParseFiredTime(AlertInstance alertInstance)
        {
            var firedTimeValue = DateTime.MinValue;
            if (alertInstance["FiredTime"] != null && !string.IsNullOrWhiteSpace(alertInstance["FiredTime"].Value))
            {
                if (!DateTime.TryParse(alertInstance["FiredTime"].Value, out firedTimeValue))
                {
                    firedTimeValue = DateTime.MinValue;
                }
            }

            if (firedTimeValue != DateTime.MinValue)
            {
                return;
            }

            if (alertInstance["FiredTime"] == null)
            {
                alertInstance.AddProperty("FiredTime", string.Empty);
            }

            firedTimeValue = DateTime.UtcNow;

            alertInstance["FiredTime"].Value = firedTimeValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}
