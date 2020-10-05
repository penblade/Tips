using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tips.Pipeline;

namespace Tips.Middleware.ErrorHandling
{
    public class ProblemDetailsWithNotifications : ProblemDetails
    {
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
