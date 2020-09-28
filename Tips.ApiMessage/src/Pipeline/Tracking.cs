using System.Diagnostics;

namespace Tips.Pipeline
{
    internal static class Tracking
    {
        public static string TraceId => Activity.Current?.Id;
    }
}
