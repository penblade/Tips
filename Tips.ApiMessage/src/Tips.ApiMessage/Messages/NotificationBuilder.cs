namespace Tips.ApiMessage.Messages
{
    public class NotificationBuilder
    {
        private readonly Notification _notification;

        public NotificationBuilder()
        {
            _notification = new Notification();
        }

        public NotificationBuilder Id(string id) { _notification.Id = id; return this; }
        public NotificationBuilder Severity(Severity severity) { _notification.Severity = severity; return this; }
        public NotificationBuilder Detail(string detail) { _notification.Detail = detail; return this; }
        public Notification Build() => _notification;
    }
}
