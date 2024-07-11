using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Configuration;

namespace LogCollector.LogWriters;

internal class AppInsightWriterOptions : WriterOptionsBase
{
    public required string ConnectionString { get; set; }
}

internal class AppInsightWriter : WriterBase<AppInsightWriterOptions>
{
    public AppInsightWriter(ILogCollector[] collectors, IConfiguration configuration)
        : base(collectors, configuration)
    {
        m_appInsight = new TelemetryClient(new TelemetryConfiguration()
        {
            ConnectionString = Options.ConnectionString,
        });
    }

    public override void Write(TraceEvent traceEvent)
    {
        m_appInsight.TrackTrace(traceEvent.FormattedMessage);
    }

    private readonly TelemetryClient m_appInsight;
}
