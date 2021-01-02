using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Tips.Pipeline
{
    public static class Tracking
    {
        // https://www.w3.org/TR/trace-context/
        // TraceParentStateString and TraceStateString values are not validated here.
        // If these fields are critical for your tracking, then follow the detailed guidelines in the W3C trace-context.
        public static string TraceParentId => Activity.Current?.ParentId;
        public static string TraceId => Activity.Current?.Id;
        public static string TraceParentStateString => Activity.Current?.TraceStateString;
        public static string TraceStateString(string traceStateStringValue) => $"{ApplicationName}={traceStateStringValue}{ParseTraceParentStateString()}";

        private static readonly string ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name?.ToLower();
        private static string ParseTraceParentStateString()
        {
            var state = TraceParentStateString?.Split(',').ToList().FirstOrDefault();
            return !string.IsNullOrEmpty(state) ? $",{state}" : string.Empty;
        }
    }
}
