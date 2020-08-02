using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tips.ApiMessage.Contracts;

namespace Tips.ApiMessage.Error.Controllers
{
    [ApiController]
    public class ErrorController : Controller
    {
        // TODO: Add Middleware exception handler.  While this works, it just feels like a code smell having a global exception handler as an endpoint.
        // Now that I've added the logger, I seem to be recording the global exception twice.  Need to research.

        private readonly ILogger _logger;

        public ErrorController(ILogger<ErrorController> logger) =>_logger = logger;

        // https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1
        [Route("/error")]
        public IActionResult Error()
        {
            using var scope = _logger.BeginScope(nameof(Error));

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;

            // ProblemDetails implements the RF7807 standards.
            // TraceId is already returned.
            var problemDetails = new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "Internal Server Error",
                Instance = $"urn:{CompanyConstants.UrnName}:error:{Guid.NewGuid()}",
            };

            _logger.LogError(exception, "Uncaught Exception", exception);

            return Problem(problemDetails.Detail, problemDetails.Instance, problemDetails.Status, problemDetails.Title, problemDetails.Type);
        }
    }
}