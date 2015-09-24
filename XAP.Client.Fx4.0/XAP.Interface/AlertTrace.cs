using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XAP.Interface
{
    public class AlertTrace
    {
        public DateTime Datetime { get; set; }
        public string Message { get; set; }

        public string Source { get; set; }

        public TraceLevel Level { get; set; }
    }
}
