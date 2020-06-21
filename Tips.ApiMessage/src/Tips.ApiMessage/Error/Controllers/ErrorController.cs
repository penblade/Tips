using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tips.ApiMessage.Contracts;

namespace Tips.ApiMessage.Error.Controllers
{
    [ApiController]
    public class ErrorController : Controller
    {
        // https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-3.1
        [Route("/error")]
        public IActionResult Error()
        {
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

            // TODO: log the exception

            return Problem(problemDetails.Detail, problemDetails.Instance, problemDetails.Status, problemDetails.Title, problemDetails.Type);
        }
    }
}