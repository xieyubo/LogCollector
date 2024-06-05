using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LogCollector;

internal class Worker : BackgroundService, IDisposable
{
    public Worker(IServiceProvider sp)
    {
        var config = sp.GetRequiredService<IConfiguration>().GetRequiredSection("LogCollector");
        _collectors = CreateObjects<ILogCollector>(sp, config.GetRequiredSection("collectors"));
        _writters = CreateObjects<ILogWritter>(new LogWritterServiceProviderWrapper(sp, _collectors), config.GetRequiredSection("writters"));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _collectors.ForEach(c => c.Start());
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _collectors.ForEach(c => c.Stop());

        foreach (var writter in _writters)
        {
            (writter as IDisposable)?.Dispose();
        }

        foreach (var collector in _collectors)
        {
            (collector as IDisposable)?.Dispose();
        }

        base.Dispose();
    }

    private class LogWritterServiceProviderWrapper : IServiceProvider
    {
        public LogWritterServiceProviderWrapper(IServiceProvider defaultServiceProvider, ILogCollector[] collectors)
        {
            _defaultServiceProvider = defaultServiceProvider;
            _collectors = collectors;
        }

        public object? GetService(Type serviceType)
        {
            return serviceType.IsAssignableFrom(typeof(ILogCollector[])) ? _collectors : _defaultServiceProvider.GetService(serviceType);
        }

        private readonly IServiceProvider _defaultServiceProvider;
        private readonly ILogCollector[] _collectors;
    }

    private static T[] CreateObjects<T>(IServiceProvider sp, IConfiguration config)
    {
        var types = Assembly
            .GetAssembly(typeof(T))!
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(T).IsAssignableFrom(t))
            .ToDictionary(t => t.Name, t => t);

        return config.GetChildren().Select(s =>
        {
            var type = s.GetRequiredSection("type").Value;
            if (string.IsNullOrEmpty(type) || !types.TryGetValue(type, out var targetType) || targetType == null)
            {
                throw new Exception($"Can't find '{type}'.");
            }
            return (T)ActivatorUtilities.CreateInstance(sp, targetType, s);
        }).ToArray();
    }

    private ILogCollector[] _collectors;
    private ILogWritter[] _writters;
}
