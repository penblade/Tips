using System;
using Microsoft.Extensions.Logging;

namespace Tips.Pipeline.Extensions
{
    public static class LoggerExtensions
    {
        public static IDisposable BeginScopeWithApiTraceParentId(this ILogger logger) => logger.BeginScope("{Api.TraceParentId}", Tracking.TraceParentId);
        public static IDisposable BeginScopeWithApiTraceParentStateString(this ILogger logger) => logger.BeginScope("{Api.TraceParentStateString}", Tracking.TraceParentStateString);

        public static IDisposable BeginScopeWithApiTraceId(this ILogger logger) => logger.BeginScope("{Api.TraceId}", Tracking.TraceId);
        public static IDisposable BeginScopeWithApiTraceStateString(this ILogger logger, string traceStateStringValue) =>
            logger.BeginScope("{Api.TraceStateString}", Tracking.TraceStateString(traceStateStringValue));
        public static IDisposable BeginScopeWithApiTraceStateStringEncoded(this ILogger logger, string traceStateStringValue) =>
            logger.BeginScope("{Api.TraceStateString}", Tracking.TraceStateString(EncodeTraceStateStringValue(traceStateStringValue)));

        public static IDisposable BeginScopeWithApiScope(this ILogger logger, string scope) => logger.BeginScope("{Api.Scope}", scope);

        private static string EncodeTraceStateStringValue(string traceStateStringValue) => Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(traceStateStringValue));
    }
}
