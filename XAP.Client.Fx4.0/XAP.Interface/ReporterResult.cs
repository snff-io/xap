using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
