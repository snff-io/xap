using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IScheduledActionManager
    {
        void Start(IEnumerable<ScheduledActionConfiguration> actions, IContext context);

        void Stop();
    }
}
