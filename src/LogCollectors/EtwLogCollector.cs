using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace LogCollector.LogCollectors;

internal class EtwLogCollector : ILogCollector, IDisposable
{
    public EtwLogCollector(IConfiguration configuration)
    {
        _options = configuration.Get<Options>() ?? throw new Exception("Can't get EtwLogCollector settings.");
    }

    public event LogReceivedHandler? OnLogReceived;

    public string Name => _options.Name;

    public void Start()
    {
        if (_session == null)
        {
            lock (this)
            {
                if (_session == null)
                {
                    Task.Run(() =>
                    {
                        _session = new TraceEventSession(_options.SessionName);
                        _session.Source.Dynamic.AddCallbackForProviderEvents((_, _) => EventFilterResponse.AcceptEvent, (e) => OnLogReceived?.Invoke(e));
                        _options.Providers.ForEach(p =>
                        {
                            _session.EnableProvider(p.Guid, p.Level);
                        });
                        _session.Source.Process();
                    });
                }
            }
        }
    }

    public void Stop()
    {
        if (_session != null)
        {
            lock (this)
            {
                if (_session != null)
                {
                    _session.Stop();
                    _session.Dispose();
                    _session = null;
                }
            }
        }
    }

    public void Dispose()
    {
        Stop();
    }

    private record Porviders(string Name, Guid Guid, TraceEventLevel Level);

    private class Options
    {
        public required string Type { get; set; }
        public required string Name { get; set; }
        public required string SessionName { get; set; }
        public required Porviders[] Providers { get; set; }
    }

    private Options _options;
    private TraceEventSession? _session;
}
