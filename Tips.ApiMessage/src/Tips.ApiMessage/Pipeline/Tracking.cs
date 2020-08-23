using System.Diagnostics;

namespace Tips.ApiMessage.Pipeline
{
    public static class Tracking
    {
        public static string TraceId => Activity.Current?.Id;
    }
}
