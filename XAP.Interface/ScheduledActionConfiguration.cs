using System;
using System.Collections.Generic;

namespace XAP.Interface
{
    public class ScheduledActionConfiguration
    {
        public ScheduledActionConfiguration()
        {
            this.Params = new Dictionary<string, string>();
        }

        public TimeSpan Interval { get; set; }

        public string Assembly { get; set; }

        public string Type { get; set; }

        public Dictionary<string, string> Params { get; set; }
    }
}
