using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Tips.Pipeline;

namespace Tips.Middleware
{
    internal class ProblemDetailFactory : IProblemDetailFactory
    {
        private readonly ProblemDetailConfiguration _configuration;

        public ProblemDetailFactory(ProblemDetailConfiguration configuration) => _configuration = configuration;

        public ProblemDetailsWithNotifications BadRequest(List<Notification> notifications)
        {
            const string badRequestId = "69244389-3C4E-4D94-ABDC-C05E703E3DBD";

            // ProblemDetails implements the RF7807 standards.
            var problemDetails = new ProblemDetailsWithNotifications
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Review the notifications for details.",
                Instance = $"urn:{_configuration.UrnName}:error:{badRequestId}",
                Notifications = notifications
            };

            problemDetails.Extensions["traceId"] = Tracking.TraceId;
            return problemDetails;
        }

        public ProblemDetails InternalServerError()
        {
            const string uncaughtExceptionId = "D1537B75-D85A-48CF-8A02-DF6C614C3198";

            // ProblemDetails implements the RF7807 standards.
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "Internal Server Error",
                Instance = $"urn:{_configuration.UrnName}:error:{uncaughtExceptionId}"
            };

            problemDetails.Extensions["traceId"] = Tracking.TraceId;
            return problemDetails;
        }
    }
}
