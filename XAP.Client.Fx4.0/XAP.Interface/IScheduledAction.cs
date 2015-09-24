using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IScheduledAction
    {
        void Invoke(Dictionary<string, string> parameters, IContext context);

    }
}
