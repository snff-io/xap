using System;
using System.Diagnostics;

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
