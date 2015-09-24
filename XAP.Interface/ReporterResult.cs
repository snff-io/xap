using System.Collections.Generic;

namespace XAP.Interface
{
    public class ReporterResult
    {
        public ReporterResult()
        {
            this.Alerts = new List<AlertInstance>();
        }
        public ICollection<AlertInstance> Alerts { get; set; }
        public string NewCheckpoint { get; set; }
    }
}
