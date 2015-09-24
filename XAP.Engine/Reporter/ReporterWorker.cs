using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using XAP.Common;
using Xap.Engine;
using XAP.Interface;

namespace XAP.Engine.Reporter
{
    public class ReporterWorker : IReporterWorker
    {
        private IContext _context;
        private bool _run;
        private int _pushRunningCount;
        private bool _pullRunning;
        private Guid _activityGuid;

        public void Run(IContext context)
        {
            _run = true;
            _context = context;

            _activityGuid = Guid.NewGuid();
            Trace.CorrelationManager.ActivityId = _activityGuid;
            Tracing.XapTrace.TraceEvent(TraceEventType.Start, 1, "reporter worker");

            try
            {
                Tracing.XapTrace.TraceInformation("initializing push reporters");

                foreach (var reporter in _context.Reporters.OfType<IReporterPush>())
                {
                    Tracing.XapTrace.TraceInformation("init push reporter {0}", reporter.FriendlyName);
                    if (!_run)
                    {
                        Tracing.XapTrace.TraceInformation("break during push reporter init");
                        break;
                    }

                    Action<IEnumerable<AlertInstance>> push = a =>
                    {
                        Interlocked.Increment(ref _pushRunningCount);
                        try
                        {
                            a.ForEach(x =>
                            {
                                _context.Factory.ParseFiredTime(x);
                                Enqueue(x);
                            });
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _pushRunningCount);
                        }
                    };

                    Tracing.XapTrace.TraceInformation("initializing push reporter " + reporter.FriendlyName);
                    reporter.Initialize(push, _context);
                }
            }
            catch (Exception ex)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Critical, 11, "reporter worker::run" + ex.ToString());
                throw;
            }


            while (true)
            {
                Thread.Sleep(5000);
                if (!_run)
                {
                    break;
                }

                _pullRunning = true;
                try
                {
                    foreach (var reporter in _context.Reporters.OfType<IReporterPull>().TakeWhile(reporter => _run))
                    {
                        Guid lockToken = Guid.Empty;
                        string lockData = String.Empty;
                        try
                        {
                            Tracing.XapTrace.TraceEvent(TraceEventType.Verbose, 0, "trying lock on reporter " + reporter.Key);

                            if (_context.CheckpointManger.TryGetLock(reporter.Key, out lockToken, out lockData))
                            {
                                Tracing.XapTrace.TraceEvent(TraceEventType.Verbose, 0, "lock granted on reporter, getting alerts " + reporter.Key);

                                var res = reporter.GetAlerts(lockData, _context);

                                Tracing.XapTrace.TraceEvent(TraceEventType.Verbose, 0, res.Alerts.Count() + " alerts gathered from " + reporter.Key);
                                Tracing.XapTrace.TraceEvent(TraceEventType.Verbose, 0, reporter.Key + " setting checkpoint " + res.NewCheckpoint);
                                lockData = res.NewCheckpoint;

                                if (res.Alerts == null || !res.Alerts.Any())
                                {
                                    continue;
                                }

                                res.Alerts.ForEach(x =>
                                {
                                    Enqueue(x);
                                });
                                Tracing.XapTrace.TraceEvent(TraceEventType.Verbose, 0, res.Alerts.Count() + "reporter queued alerts " + reporter.Key);
                            }
                            else
                            {
                                Tracing.XapTrace.TraceEvent(TraceEventType.Verbose, 0, "no lock granted for reporter " + reporter.Key);
                            }
                        }
                        finally
                        {
                            if (!lockToken.Equals(Guid.Empty))
                            {
                                Tracing.XapTrace.TraceEvent(TraceEventType.Verbose, 0, "releasing lock on reporter " + reporter.Key);
                                _context.CheckpointManger.TryReleaseLock(reporter.Key, lockToken, lockData);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Tracing.XapTrace.TraceEvent(TraceEventType.Error, -999, ex.ToString());
                    throw;
                }
                finally
                {
                    _pullRunning = false;
                }
            }

            Tracing.XapTrace.TraceEvent(TraceEventType.Stop, -999, "reporter worker stopped");
        }

        private void Enqueue(AlertInstance x)
        {
            x.Reported = DateTime.UtcNow;
            TokenParser.ProcessTitleTokens(x);
            //x.SetNewLine(Utility.XapNewLine);
            x.AddTrace(TraceEventType.Start, 15, "{0}:{1}:{2}", DateTime.UtcNow, x.Reporter, x.XapId);
            x.TraceProperties();
            _context.AlertQueue.Enqueue(x);
        }

        public bool BlockingStopAll(TimeSpan timeout)
        {
            _run = false;
            var start = DateTime.UtcNow;

            while (_pullRunning || _pushRunningCount > 0)
            {
                if (DateTime.UtcNow.Subtract(start) > timeout)
                {
                    return false;
                }

                Thread.Sleep(100);
            }

            _context.Reporters.ForEach(x => x.Dispose());

            return true;
        }
    }
}
