using Microsoft.Diagnostics.Tracing;

namespace LogCollector;

internal delegate void LogReceivedHandler(TraceEvent traceEvent);

internal interface ILogCollector
{
    event LogReceivedHandler OnLogReceived;

    string Name { get; }

    void Start();

    void Stop();
}
