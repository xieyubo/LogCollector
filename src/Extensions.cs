using Microsoft.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LogCollector;

internal static class Extensions
{
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> act)
    {
        foreach (var item in items)
        {
            act(item);
        }
    }

    public static LogLevel ToLogLevel(this TraceEventLevel level)
    {
        return level switch
        {
            TraceEventLevel.Verbose => LogLevel.Debug,
            TraceEventLevel.Informational => LogLevel.Information,
            TraceEventLevel.Warning => LogLevel.Warning,
            TraceEventLevel.Error => LogLevel.Error,
            TraceEventLevel.Critical => LogLevel.Critical,
            TraceEventLevel.Always => LogLevel.Critical,
            _ => LogLevel.None,
        };
    }
}
