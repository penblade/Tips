using System.Collections.Generic;
using System.Linq;

namespace Tips.Pipeline
{
    public class Response<TItem> : Response
    {
        public TItem Item { get; set; }
    }

    public class Response
    {
        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public void Add(Notification notification) => Notifications.Add(notification);
        public bool HasErrors() => Notifications.Any(notification => notification.Severity == Notification.SeverityType.Error);
        public bool IsNotFound() => Notifications.Any(notification => notification is NotFoundNotification);
    }
}
