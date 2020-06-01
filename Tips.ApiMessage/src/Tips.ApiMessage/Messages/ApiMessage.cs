using System.Collections.Generic;

namespace Tips.ApiMessage.Messages
{
    public class ApiMessage
    {
        public string TraceId { get; set; }
        public IEnumerable<Notification> Notifications { get; set; } = new List<Notification>();
        public int Status { get; set; }
    }
}
