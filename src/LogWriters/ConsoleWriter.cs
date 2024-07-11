using LogCollector.LogWriters;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace LogCollector;

internal class ConsoleWriter : WriterBase, IDisposable
{
    public ConsoleWriter(ILogger<ConsoleWriter> logger, ILogCollector[] collectors, IConfiguration configuration)
        : base(collectors, configuration)
    {
        m_logger = logger;
    }

    public override void Write(TraceEvent traceEvent)
    {
        m_logger.Log(traceEvent.Level.ToLogLevel(), traceEvent.FormattedMessage);
    }

    private readonly ILogger m_logger;
}
