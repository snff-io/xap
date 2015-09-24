using System;
using System.Diagnostics;
using System.Threading;
using XAP.Common;
using XAP.Interface;

namespace XAP.Engine.Router
{
    public class RouterWorker : IRouterWorker
    {
        private IAlertQueue _queue;
        private IRouteManager _routeManager;
        private IContext _context;
        private IWorkerFactory _factory;
        private bool _run;
        private PerformanceCounter _totalCounter;
        private PerformanceCounter _rateCounter;
        private PerformanceCounter _errorCounter;

        public void Initialize(IContext context)
        {
            _context = context;
            _factory = _context.Factory;
            _queue = _context.AlertQueue;
            _routeManager = _context.RouteManager;

            InitPerf();
        }

        private void InitPerf()
        {
            try
            {
                if (PerformanceCounterCategory.Exists("XapRouterWorker"))
                {
                    PerformanceCounterCategory.Delete("XapRouterWorker");
                }

                PerformanceCounterCategory.Create("XapRouterWorker", "XAP Router Worker Counters",
                    PerformanceCounterCategoryType.SingleInstance, new CounterCreationDataCollection(
                        new[]
                        {
                            new CounterCreationData
                            {
                                CounterName = "Routed_Total",
                                CounterHelp = "How many alerts have been routed in total since the last restart",
                                CounterType = PerformanceCounterType.NumberOfItems64
                            },

                            new CounterCreationData
                            {
                                CounterName = "Routed_PerSecond",
                                CounterHelp = "Alerts routed per second",
                                CounterType = PerformanceCounterType.RateOfCountsPerSecond64
                            },

                            new CounterCreationData
                            {
                                CounterName = "Route_Errors_PerSecond",
                                CounterHelp = "Number of errors encountered while processing alerts",
                                CounterType = PerformanceCounterType.RateOfCountsPerSecond64
                            }
                        }));
            }
            catch (Exception ex)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Error, 0, "Error creating counters, counters may not be available." + ex);
            }

            _rateCounter = new PerformanceCounter("XapRouterWorker", "Routed_PerSecond", false);
            _totalCounter = new PerformanceCounter("XapRouterWorker", "Routed_Total", false);
            _errorCounter = new PerformanceCounter("XapRouterWorker", "Route_Errors_PerSecond", false);
        }

        public void Run()
        {
            _run = true;

            while (_run)
            {
                var alertInstance = _queue.StartNext();
                //alertInstance.SetNewLine(Utility.StandardNewLine);
                if (alertInstance != null)
                {
                    RouteInstance(alertInstance);
                }

                Thread.Sleep(500);
            }
        }

        public void Stop()
        {
            _run = false;
        }

        public void RouteInstance(AlertInstance alert)
        {
            try
            {
                alert.AddTrace("Started alert route processing on server {0}", Environment.MachineName);
                RouterResponse route = _routeManager.Match(alert);

                if (route == null)
                {
                    alert.AddTrace(TraceEventType.Error, -999, "Unable to route the alert. No routes found that match the current alert.");
                    return;
                }
                foreach (var routeMany in route.RouteManyRoutes)
                {
                    foreach (var action in routeMany.Actions)
                    {
                        alert.AddTrace("Running action '{0}' for route '{1}", action.Type, routeMany.Name);
                        try
                        {
                            var actionInstance = _factory.CreateAlertActionInstance(action.Type, action.Params);
                            var result = actionInstance.PerformAction(alert, _context);
                            alert.AddTrace("'{0}' completed with a result of '{1}'", action.Type, result);

                            if (result == AlertActionResult.FailStop)
                            {
                                _errorCounter.Increment();
                                _context.AlertQueue.Enqueue(alert);
                                return;
                            }
                            else if (result == AlertActionResult.OkStop)
                            {
                                _context.AlertQueue.End(alert.XapId);
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            alert.AddTrace(TraceEventType.Error, 0, "An Unhandled Exception occurred while processing action {0}: {1}", action.Type, e);
                        }
                        finally
                        {
                            _context.Persistence.PersistAlert(alert);
                        }
                    }
                }

                alert.AddTrace("Re-evaluating routes");
                route = _routeManager.Match(alert);

                if (route.RouteOneRoute == null)
                {
                    return;
                }

                foreach (var action in route.RouteOneRoute.Actions)
                {
                    try
                    {
                        alert.AddTrace("Running action '{0}' for route '{1}", action.Type, route.RouteOneRoute.Name);

                        var actionInstance = _factory.CreateAlertActionInstance(action.Type, action.Params);
                        var result = actionInstance.PerformAction(alert, _context);
                        alert.AddTrace("'{0}' completed with a result of '{1}'", action.Type, result);

                        if (result == AlertActionResult.OkContinue)
                        {
                            _context.AlertQueue.End(alert.XapId);
                        }
                        else if (result == AlertActionResult.FailContinue)
                        {
                            _context.AlertQueue.Enqueue(alert);
                            _errorCounter.Increment();
                        }
                        else if (result == AlertActionResult.FailStop)
                        {
                            _context.AlertQueue.Enqueue(alert);
                            _errorCounter.Increment();
                            break;
                        }
                        else if (result == AlertActionResult.OkStop)
                        {
                            _context.AlertQueue.End(alert.XapId);
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        alert.AddTrace(TraceEventType.Error, 0, "An Unhandled Exception occurred while processing action {0}: {1}", action.Type, e);
                        _errorCounter.Increment();
                    }
                    finally
                    {
                        _context.Persistence.PersistAlert(alert);
                    }
                }
            }
            finally
            {
                _rateCounter.Increment();
                _totalCounter.Increment();
            }
        }
    }
}
