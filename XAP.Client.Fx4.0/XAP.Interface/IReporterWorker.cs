using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Interface
{
    public interface IReporterWorker
    {
        void Run(IContext context);

        bool BlockingStopAll(TimeSpan timeout);
        
    }
}
