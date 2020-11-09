using System;
using Microsoft.Extensions.Logging;

namespace Tips.Pipeline.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogAction(this ILogger logger, string scope, Action method)
        {
            using (logger.BeginScopeWithApiTraceParentId())
            using (logger.BeginScopeWithApiTraceParentStateString())
            using (logger.BeginScopeWithApiTraceId())
            using (logger.BeginScopeWithApiTraceStateString(scope))
            using (logger.BeginScopeWithApiScope(scope))
            {
                method();
            }
        }

        private static IDisposable BeginScopeWithApiTraceParentId(this ILogger logger) =>
            logger.BeginScope("{Api.TraceParentId}", Tracking.TraceParentId);

        private static IDisposable BeginScopeWithApiTraceParentStateString(this ILogger logger) =>
            logger.BeginScope("{Api.TraceParentStateString}", Tracking.TraceParentStateString);

        private static IDisposable BeginScopeWithApiTraceId(this ILogger logger) =>
            logger.BeginScope("{Api.TraceId}", Tracking.TraceId);

        private static IDisposable BeginScopeWithApiTraceStateString(this ILogger logger, string traceStateStringValue) =>
            logger.BeginScope("{Api.TraceStateString}", Tracking.TraceStateString(traceStateStringValue));

        private static IDisposable BeginScopeWithApiTraceStateStringEncoded(this ILogger logger, string traceStateStringValue) =>
            logger.BeginScope("{Api.TraceStateString}", Tracking.TraceStateString(EncodeTraceStateStringValue(traceStateStringValue)));

        private static IDisposable BeginScopeWithApiScope(this ILogger logger, string scope) =>
            logger.BeginScope("{Api.Scope}", scope);

        private static string EncodeTraceStateStringValue(string traceStateStringValue) =>
            Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(traceStateStringValue));
    }
}
