using System.Diagnostics;

namespace Tips.Middleware
{
    internal static class Tracking
    {
        public static string TraceId => Activity.Current?.Id;
    }
}
