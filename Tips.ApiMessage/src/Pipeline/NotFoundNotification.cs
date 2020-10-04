namespace Tips.Pipeline
{
    /// <summary>
    /// Use this special notification type to denote that the item was not found
    /// when getting, updating, or deleting a single item.
    /// </summary>
    public class NotFoundNotification : Notification
    {
        public static Notification Create(string id, string detail) => new NotFoundNotification { Id = id, Detail = detail, Severity = SeverityType.Error };
    }
}
