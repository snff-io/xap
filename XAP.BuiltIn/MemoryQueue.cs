using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XAP.Common;
using XAP.Interface;


namespace XAP.BuiltIn
{
    public class MemoryQueue : IAlertQueue
    {
        private readonly List<AlertInstance> _queue;
        private readonly Dictionary<Guid, DateTime> _inProgress; 

        private readonly object _queueLock = new object();

        public MemoryQueue()
        {
            _queue = new List<AlertInstance>();
            _inProgress = new Dictionary<Guid, DateTime>();
        }

        public void Enqueue(AlertInstance alert)
        {
            try
            {
                lock (_queueLock)
                {
                    if (_queue.All(a => a.XapId != alert.XapId))
                    {
                        _queue.Add(alert);
                    }
                }
            }
            catch (Exception e)
            {
                Tracing.XapTrace.TraceEvent(TraceEventType.Error, -999, e.ToString());
            }
        }

        public void Enqueue(IEnumerable<AlertInstance> alerts)
        {
            alerts.ForEach(this.Enqueue);
        }

        public AlertInstance StartNext()
        {
            lock (_queueLock)
            {
                DateTime staleAge = DateTime.UtcNow.AddMinutes(-15);

                // grabs all items in the queue that are not in the 'in progress' list and are still newer than the stale age.
                // ordered by alert priority
                var candidates =
                    _queue.OrderBy(a => a["Priority"].Value).ThenBy(a => DateTime.Parse(a["FiredTime"].Value))
                        .Where(item => !_inProgress.ContainsKey(item.XapId) || _inProgress[item.XapId] <= staleAge);

                foreach (var alert in candidates)
                {
                    if (_inProgress.ContainsKey(alert.XapId))
                    {
                        _inProgress[alert.XapId] = DateTime.UtcNow;
                    }
                    else
                    {
                        _inProgress.Add(alert.XapId, DateTime.UtcNow);
                    }

                    return alert;
                }
            }

            return null;
        }

        public void End(Guid xapId)
        {
            if (_queue.All(a => a.XapId != xapId) && _inProgress.ContainsKey(xapId))
            {
                return;
            }

            lock (_queueLock)
            {
                var item = _queue.SingleOrDefault(a => a.XapId == xapId);
                if (item != null)
                {
                    _queue.Remove(item);
                }

                var hasInProgressItem = _inProgress.ContainsKey(xapId);

                if (hasInProgressItem)
                {
                    _inProgress.Remove(xapId);
                }
            }
        }

        public AlertInstance Peek()
        {
            AlertInstance alert = _queue.OrderBy(a => a["Priority"].Value).ThenBy(a => DateTime.Parse(a["FiredTime"].Value)).FirstOrDefault();
            return alert;
        }

        public int Count()
        {
            return _queue.Count;
        }
    }
}
