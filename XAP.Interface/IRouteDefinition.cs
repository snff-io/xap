using System.Collections.Generic;

namespace XAP.Interface
{
    public interface IRouteDefinition
    {
        IEnumerable<IAlertAction> Actions {get;}
    }
}
