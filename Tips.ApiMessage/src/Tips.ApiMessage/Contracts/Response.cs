using System.Collections.Generic;
using Tips.ApiMessage.Pipeline;

namespace Tips.ApiMessage.Contracts
{
    public class Response<TResult> : Response
    {
        public TResult Result { get; set; }
    }

    public class Response
    {
        public IEnumerable<Notification> Notifications { get; set; } = new List<Notification>();
        public int Status { get; set; }
        public string TraceId => Tracking.TraceId;
    }
}
