using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using XAP.Common;
using XAP.Interface;

namespace XAP.Engine
{
    public class ScheduledActionManager : IScheduledActionManager
    {
        List<Timer> _timers;
        Dictionary<ScheduledActionConfiguration, IScheduledAction> _configuredActions;
        private IContext _context;
        private readonly Guid _activityId = Guid.NewGuid();

        public void Start(IEnumerable<ScheduledActionConfiguration> actions, IContext context)
        {
            _context = context;

            Tracing.XapTrace.TraceInformation("setting up scheduled actions");

            _timers = new List<Timer>();
            _configuredActions = new Dictionary<ScheduledActionConfiguration, IScheduledAction>();

            foreach (var actionConfig in actions)
            {
                var aInstance = context.Factory.LoadInstanceFromAssembly<IScheduledAction>(new SimpleTypeDefinition
                {
                    AssemblyName = actionConfig.Assembly,
                    TypeName = actionConfig.Type
                });

                _configuredActions[actionConfig] = aInstance;

                var timer = new Timer(this.Tick, actionConfig, 0, (long)actionConfig.Interval.TotalMilliseconds);
                Tracing.XapTrace.TraceInformation("scheduled action setup: {0}, Interval:{1}", actionConfig.Type, actionConfig.Interval.TotalMilliseconds);
                _timers.Add(timer);
            }
        }

        private void Tick(object state)
        {
            var config = state as ScheduledActionConfiguration;
            if (config == null)
            {
                return;
            }
            var actionInstance = _configuredActions[config];
            
            var previousActivity = Trace.CorrelationManager.ActivityId;
            Trace.CorrelationManager.ActivityId = _activityId;
            try
            {
                actionInstance.Invoke(config.Params, _context);
            }
            catch (Exception e)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Error, -999, "scheduled action tick failed: {0}, {1}", config.Type, e);
            }
            finally
            {
                Trace.CorrelationManager.ActivityId = previousActivity;
            }
        }

        public void Stop()
        {
            foreach (var t in _timers)
            {
                t.Dispose();
            }
        }
    }
}
