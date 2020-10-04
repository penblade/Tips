using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tips.Pipeline;

namespace Tips.Middleware
{
    public interface IProblemDetailFactory
    {
        ProblemDetailsWithNotifications BadRequest(List<Notification> notifications);
        ProblemDetails InternalServerError();
    }
}
