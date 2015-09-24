using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using XAP.Interface;

namespace XAP.BuiltIn
{
    public class WriteEventLogAction:IAlertAction
    {
        public AlertActionResult PerformAction(AlertInstance alert, IContext config)
        {
            if (!EventLog.SourceExists("WriteEventLogAction"))
            {
                EventLog.CreateEventSource("WriteEventLogAction", "XAP");
            }

            var stringBuilder = new StringBuilder();

            foreach (var property in alert.Properties.OrderBy(x=>x.Name))
            {
                stringBuilder.AppendLine(property.Name + ": " + property.Value);
            }

            EventLog.WriteEntry("WriteEventLogAction", stringBuilder.ToString(), EventLogEntryType.Information);

            return AlertActionResult.OkContinue;
        }

        public void SetParameters(Dictionary<string, string> parameters)
        {
            
        }
    }
}
