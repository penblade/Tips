using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tips.ApiMessage.Handlers
{
    public class ActionResultHandler<TRequest, TResponse> : IActionResultHandler<TRequest, TResponse> where TResponse : Response
    {
        public async Task<ActionResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var response = await next();
                return response?.ApiMessage?.Status switch
                {
                    null => throw new Exception("HttpStatusCode was not set."),
                    (int) HttpStatusCode.OK => new OkObjectResult(response),
                    (int) HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
                    (int)HttpStatusCode.NotFound => new NotFoundObjectResult(response),
                    (int) HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(response),
                    _ => throw new Exception($"HttpStatusCode {response.ApiMessage.Status} was not handled.")
                };
            }
            catch (Exception ex)
            {
                // TODO: Log exception
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
