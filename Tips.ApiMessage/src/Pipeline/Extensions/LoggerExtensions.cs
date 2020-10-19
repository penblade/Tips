using System;
using Microsoft.Extensions.Logging;

namespace Tips.Pipeline.Extensions
{
    public static class LoggerExtensions
    {
        public static IDisposable BeginScopeWithApiTraceId(this ILogger logger) => logger.BeginScope("{Api.TraceId}", Tracking.TraceId);
        public static IDisposable BeginScopeWithApiScope(this ILogger logger, string scope) => logger.BeginScope("{Api.Scope}", scope);
    }
}
