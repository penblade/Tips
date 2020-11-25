using System.Collections.Generic;

namespace Tips.Pipeline
{
    public class Notification
    {
        public string Id { get; set; }
        public string Severity { get; set; }
        public string Detail { get; set; }

        public List<Notification> Notifications { get; } = new List<Notification>();

        public static Notification CreateError(string id, string detail) => new Notification { Id = id, Detail = detail, Severity = SeverityType.Error };
        public static Notification CreateInfo(string id, string detail) => new Notification { Id = id, Detail = detail, Severity = SeverityType.Info };
        public static Notification CreateWarning(string id, string detail) => new Notification { Id = id, Detail = detail, Severity = SeverityType.Warning };

        public static class SeverityType
        {
            public const string Error = "Error";
            public const string Info = "Info";
            public const string Warning = "Warning";
        }
    }
}
