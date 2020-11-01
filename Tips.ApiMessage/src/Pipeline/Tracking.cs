using System.Diagnostics;

namespace Tips.Pipeline
{
    public static class Tracking
    {
        public static string TraceParentId => Activity.Current?.ParentId;
        public static string TraceId => Activity.Current?.Id;
    }
}
