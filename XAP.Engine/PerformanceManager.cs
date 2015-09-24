using System.Diagnostics;
using System.Threading;
using XAP.Interface;

namespace XAP.Engine
{
    public class PerformanceManager : IPerformanceManager
    {
        IAlertQueue _queue;

        public void StartLoop(IAlertQueue queue)
        {
            _queue = queue;

            if (!PerformanceCounterCategory.Exists("XAP"))
            {

                PerformanceCounterCategory.Create("XAP", "XAP Counters", PerformanceCounterCategoryType.SingleInstance, new CounterCreationDataCollection(
                     new[] 
                        {
                            new CounterCreationData
                            {
                                CounterName = "Queue_Length",
                                CounterHelp = "How many items are currently in the memory queue",
                                CounterType = PerformanceCounterType.NumberOfItems64
                            },

                            new CounterCreationData
                            {
                                CounterName = "Routed_PerSecond",
                                CounterHelp = "Alerts Routed Per Second",
                                CounterType = PerformanceCounterType.RateOfCountsPerSecond64
                            }
                        }));
            }

            var queueLenCounter = new PerformanceCounter("XAP", "Queue_Length", false);

            while (true)
            {
                Thread.Sleep(500);
                queueLenCounter.RawValue = _queue.Count();
            }
        }
    }
}
