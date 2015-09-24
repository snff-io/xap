using System;
using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IReporterPush : IReporter
    {
        void Initialize(Action<IEnumerable<AlertInstance>> pushAlertMethod, IContext context);
    }
}
