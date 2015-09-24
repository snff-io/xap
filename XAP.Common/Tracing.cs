using System.Diagnostics;

namespace XAP.Common
{
    public static class Tracing
    {
        //TODO: Nest: Tracing.Sources.Xap.TraceError(...)
        //TODO: Configurable Sources
        public static TraceSource XapTrace = new TraceSource("XapTraceSource");
        public static TraceSource AlertTrace = new TraceSource("AlertTraceSource");
    }
}
