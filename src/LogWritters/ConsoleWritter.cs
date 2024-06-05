using LogCollector.LogWritters;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace LogCollector;

internal class ConsoleWritter : WritterBase, IDisposable
{
    public ConsoleWritter(ILogger<ConsoleWritter> logger, ILogCollector[] collectors, IConfiguration configuration)
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
