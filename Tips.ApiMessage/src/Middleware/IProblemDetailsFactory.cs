using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tips.Pipeline;

namespace Tips.Middleware
{
    public interface IProblemDetailsFactory
    {
        ProblemDetailsWithNotifications BadRequest(List<Notification> notifications);
        ProblemDetails InternalServerError();
    }
}
