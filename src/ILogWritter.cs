using Microsoft.Diagnostics.Tracing;

namespace LogCollector;

internal interface ILogWritter
{
    void Write(TraceEvent traceEvent);
}
