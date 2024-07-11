using Microsoft.Diagnostics.Tracing;

namespace LogCollector;

internal interface ILogWriter
{
    void Write(TraceEvent traceEvent);
}
