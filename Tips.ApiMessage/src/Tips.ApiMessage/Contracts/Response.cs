using System.Collections.Generic;
using System.Linq;
using System.Net;
using Tips.ApiMessage.Pipeline;

namespace Tips.ApiMessage.Contracts
{
    public class Response<TResult> : Response
    {
        public TResult Result { get; set; }
    }

    public class Response
    {
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public int Status { get; set; }
        public string TraceId => Tracking.TraceId;
        public bool HasErrors() => Notifications.Any(notification => notification.Severity == Notification.SeverityType.Error);

        public void Add(Notification notification) => Notifications.Add(notification);

        public void SetStatusToBadRequest() => Status = (int) HttpStatusCode.BadRequest;
        public void SetStatusToCreated() => Status = (int) HttpStatusCode.Created;
        public void SetStatusToNoContent() => Status = (int) HttpStatusCode.NoContent;
        public void SetStatusToNotFound() => Status = (int) HttpStatusCode.NotFound;
        public void SetStatusToOk() => Status = (int) HttpStatusCode.OK;
    }
}
