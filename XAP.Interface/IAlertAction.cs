using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IAlertAction
    {
        void SetParameters(Dictionary<string, string> parameters);

        AlertActionResult PerformAction(AlertInstance alert, IContext config);
    }
}
