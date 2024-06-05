using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace LogCollector.LogWritters;

internal class WritterOptionsBase
{
    public required string Type { get; set; }
    public required string[] collectors { get; set; }
}

internal abstract class WritterBase<TWritterOptions> : ILogWritter, IDisposable
    where TWritterOptions : WritterOptionsBase
{
    protected WritterBase(ILogCollector[] collectors, IConfiguration configuration)
    {
        Options = configuration.Get<TWritterOptions>() ?? throw new Exception("Can't get ConsoleWritter options."); ;

        _collectors = Options.collectors.Select(n =>
        {
            var c = collectors.First(c => c.Name == n);
            c.OnLogReceived += Write;
            return c;
        }).ToArray();

        _collectors.ForEach(c => c.OnLogReceived += Write);
    }

    public abstract void Write(TraceEvent traceEvent);

    public void Dispose()
    {
        _collectors.ForEach(c => c.OnLogReceived -= Write);
    }

    protected TWritterOptions Options { get; }

    private ILogCollector[] _collectors { get; }
}

internal abstract class WritterBase : WritterBase<WritterOptionsBase>
{
    protected WritterBase(ILogCollector[] collectors, IConfiguration configuration)
        : base(collectors, configuration)
    {
    }
}