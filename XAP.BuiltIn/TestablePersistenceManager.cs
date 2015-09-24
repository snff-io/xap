using System;
using System.Diagnostics;
using XAP.Interface;

namespace XAP.BuiltIn
{
    public class TestablePersistenceManager:IPersistenceManager
    {
        public void PersistAlert(AlertInstance alert)
        {
            foreach (AlertProperty property in alert.Properties)
            {
                Debug.WriteLine("Persist:{0}:{1}:{2}:{3}", DateTime.UtcNow, alert.XapId, property.Name, property.Value);
            }
        }
    }
}
