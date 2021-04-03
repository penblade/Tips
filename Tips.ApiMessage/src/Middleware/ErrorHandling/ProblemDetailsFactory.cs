using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Tips.Pipeline;

namespace Tips.Middleware.ErrorHandling
{
    internal class ProblemDetailsFactory : IProblemDetailsFactory
    {
        public const string BadRequestId = "69244389-3C4E-4D94-ABDC-C05E703E3DBD";

        private readonly ProblemDetailsConfiguration _configuration;

        public ProblemDetailsFactory(ProblemDetailsConfiguration configuration) => _configuration = configuration;

        public ProblemDetailsWithNotifications BadRequest(List<Notification> notifications)
        {
            // ProblemDetails implements the RFC7807 standards.
            var problemDetails = new ProblemDetailsWithNotifications
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Review the notifications for details.",
                Instance = $"urn:{_configuration.UrnName}:error:{BadRequestId}",
                Notifications = notifications
            };

            problemDetails.Extensions["traceId"] = Tracking.TraceId;
            return problemDetails;
        }

        public ProblemDetails InternalServerError()
        {
            const string uncaughtExceptionId = "D1537B75-D85A-48CF-8A02-DF6C614C3198";

            // ProblemDetails implements the RFC7807 standards.
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
