using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace LogCollector.LogWriters;

internal class WriterOptionsBase
{
    public required string Type { get; set; }
    public required string[] collectors { get; set; }
}

internal abstract class WriterBase<TWriterOptions> : ILogWriter, IDisposable
    where TWriterOptions : WriterOptionsBase
{
    protected WriterBase(ILogCollector[] collectors, IConfiguration configuration)
    {
        Options = configuration.Get<TWriterOptions>() ?? throw new Exception("Can't get ConsoleWriter options."); ;

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

    protected TWriterOptions Options { get; }

    private ILogCollector[] _collectors { get; }
}

internal abstract class WriterBase : WriterBase<WriterOptionsBase>
{
    protected WriterBase(ILogCollector[] collectors, IConfiguration configuration)
        : base(collectors, configuration)
    {
    }
}