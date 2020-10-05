using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tips.Pipeline;

namespace Tips.Middleware.ErrorHandling
{
    public interface IProblemDetailsFactory
    {
        ProblemDetailsWithNotifications BadRequest(List<Notification> notifications);
        ProblemDetails InternalServerError();
    }
}
