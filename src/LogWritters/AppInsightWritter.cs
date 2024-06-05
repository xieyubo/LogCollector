using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Configuration;

namespace LogCollector.LogWritters;

internal class AppInsightWritterOptions : WritterOptionsBase
{
    public required string ConnectionString { get; set; }
}

internal class AppInsightWritter : WritterBase<AppInsightWritterOptions>
{
    public AppInsightWritter(ILogCollector[] collectors, IConfiguration configuration)
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
