using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using XAP.Common;
using XAP.Interface;

namespace XAP.Engine
{
    public class Worker
    {
        private WorkerFactory _factory;

        private IAlertQueue _queue;
        private IRouterWorker _router;
        private IRouteManager _routeManager;
        private IConfiguration _config;
        private IReporterWorker _reporterWorker;
        private ILockManager _checkpointManager;
        private IEnumerable<IReporter> _reporters;
        private IContext _context;
        private IPerformanceManager _perfManager;
        private IScheduledActionManager _scheduledActionManager;
        private IPersistenceManager _persistenceManager;
        private ICacheManager _cacheManager;

        private Task _routerTask;
        private Task _reporterTask;
        private Task _perfManagerTask;

        private bool _run;
        public bool Running { get; private set; }

        public void Run(IConfiguration config)
        {
            var engineActivityId = Guid.NewGuid();
            Trace.CorrelationManager.ActivityId = engineActivityId;
            Tracing.XapTrace.TraceEvent(TraceEventType.Start, 10, "engine start");

            _run = true;
            _config = config;

            try
            {
                Tracing.XapTrace.TraceInformation("calling factory ctor");
                _factory = new WorkerFactory(_config);
                _queue = _factory.CreateInstance<IAlertQueue>();
                _persistenceManager = _factory.CreateInstance<IPersistenceManager>();
                _perfManager = _factory.CreateInstance<IPerformanceManager>();
                _scheduledActionManager = _factory.CreateInstance<IScheduledActionManager>();
                _cacheManager = _factory.CreateInstance<ICacheManager>();

                if (config.WorkerMode.Contains("reporter"))
                {
                    Tracing.XapTrace.TraceInformation("launching reporter mode");
                    _checkpointManager = _factory.CreateInstance<ILockManager>();
                    _reporterWorker = _factory.CreateInstance<IReporterWorker>();
                    _reporters = _factory.CreateAllReporters();
                }

                if (config.WorkerMode.Contains("router"))
                {
                    Tracing.XapTrace.TraceInformation("launching router mode");
                    _router = _factory.CreateInstance<IRouterWorker>();
                    _routeManager = _factory.CreateInstance<IRouteManager>();
                }
            }
            catch (Exception ex)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Critical, 15, "engine worker failed to start.\r\n" + ex.ToString());
                throw;
            }

            _context = new Context
            {
                Configuration = _config,
                Factory = _factory,
                AlertQueue = _queue,
                CheckpointManger = _checkpointManager,
                Reporters = _reporters,
                Performance = _perfManager,
                Persistence = _persistenceManager,
                RouteManager = _routeManager,
                CacheManager = _cacheManager
            };

            _perfManagerTask = Task.Factory.StartNew(() => _perfManager.StartLoop(_queue));

            if (config.WorkerMode.Contains("reporter"))
            {
                Tracing.XapTrace.TraceInformation("reporter mode ON");
                StartReporter();
            }

            if (config.WorkerMode.Contains("router"))
            {
                Tracing.XapTrace.TraceInformation("router mode ON");
                StartRouter();
            }

            try
            {
                Tracing.XapTrace.TraceInformation("starting scheduled action manager");
                _scheduledActionManager.Start(_config.GetScheduledActions(), _context);
            }
            catch (Exception e)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Critical, 1, string.Format("scheduled action manager failed to start. {0}", e.ToString()));
            }

            Running = true;

            //
            //MAIN LOOP
            //

            var mainLoopActivity = Guid.NewGuid();
            Tracing.XapTrace.TraceTransfer(16, "health loop", mainLoopActivity);
            Trace.CorrelationManager.ActivityId = mainLoopActivity;

            Tracing.XapTrace.TraceEvent(TraceEventType.Start, 16, "health loop");
            while (_run)
            {
                Thread.Sleep(500);

                try
                {
                    if (config.WorkerMode.Contains("reporter") && _reporterTask.Status != TaskStatus.Running)
                    {
                        Tracing.XapTrace.TraceInformation( "restarting reporter task");
                        _reporterTask.Dispose();

                        StartReporter();
                    }

                    if (config.WorkerMode.Contains("router") && _routerTask.Status != TaskStatus.Running)
                    {
                        Tracing.XapTrace.TraceInformation("restarting router task");
                        _routerTask.Dispose();
                        StartRouter();
                    }
                }
                catch (Exception e)
                {
                    Tracing.XapTrace.TraceEvent(TraceEventType.Error, -999, "engine main loop:" + e);
                }
            }
            Tracing.XapTrace.TraceEvent(TraceEventType.Stop, 16, "health loop");
            Tracing.XapTrace.TraceTransfer(16, "stopping", engineActivityId);
            Trace.CorrelationManager.ActivityId = engineActivityId;
            
            _reporterWorker.BlockingStopAll(TimeSpan.FromMinutes(2));
            Running = false;

            Tracing.XapTrace.TraceEvent(TraceEventType.Stop, 16, "engine stop");
        }

        private void StartReporter()
        {
            try
            {
                Tracing.XapTrace.TraceInformation("starting reporter manager");
                _reporterTask = Task.Factory.StartNew(() => _reporterWorker.Run(_context));
            }
            catch (Exception ex)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Critical, 2, "reporter Manager failed to start.\r\n" + ex.ToString());
                throw;
            }
        }

        private void StartRouter()
        {
            try
            {
                Tracing.XapTrace.TraceInformation("starting router");
                _routeManager.Initialize(_context);
                _router.Initialize(_context);

                _routerTask = Task.Factory.StartNew(() => _router.Run());
            }
            catch (Exception ex)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Critical, 3, "router manager failed to start.\r\n" + ex.ToString());
                throw;
            }
        }

        public void BlockingStop(TimeSpan timeout)
        {
            var begin = DateTime.UtcNow;
            Tracing.XapTrace.TraceInformation("blocking stop called: {0}", begin);
            while (Running)
            {
                if (DateTime.UtcNow.Subtract(begin) > timeout)
                {
                    break;
                }

                Thread.Sleep(100);
            }

            Tracing.XapTrace.TraceInformation("blocking stop finished: {0}", DateTime.UtcNow);
        }
    }
}
