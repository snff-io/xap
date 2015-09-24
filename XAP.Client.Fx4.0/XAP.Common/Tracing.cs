using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAP.Common
{
    public static class Tracing
    {
        public static TraceSource XapTrace = new TraceSource("XapTraceSource");
        public static TraceSource AlertTrace = new TraceSource("AlertTraceSource");
    }
}
