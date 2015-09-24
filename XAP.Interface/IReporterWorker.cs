using System;

namespace XAP.Interface
{
    public interface IReporterWorker
    {
        void Run(IContext context);

        bool BlockingStopAll(TimeSpan timeout);
        
    }
}
