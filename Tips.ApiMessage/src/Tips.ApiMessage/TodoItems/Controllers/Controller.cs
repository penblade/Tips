using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.CreateTodoItems;

namespace Tips.ApiMessage.TodoItems.Controllers
{
    public class Controller : ControllerBase
    {
        public delegate Task<TResponse> HandleDelegate<in TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);

        protected async Task<IActionResult> Handle<TRequest, TResponse>(HandleDelegate<TRequest, TResponse> method, TRequest request, CancellationToken cancellationToken) where TResponse : Response
        {
            // TODO: Log call to service.
            // TODO: Log request if provided.
            var response = await method(request, cancellationToken);
            var actionResult = CreateActionResult(response);

            // TODO: Log actionResult
            return actionResult;
        }

        private IActionResult CreateActionResult<TResponse>(TResponse response) where TResponse : Response =>
            response?.Status switch
            {
                null => throw new Exception("HttpStatusCode was not set."),
                (int)HttpStatusCode.OK => new OkObjectResult(response),
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
                (int)HttpStatusCode.NoContent => new NoContentResult(),
                (int)HttpStatusCode.NotFound => new NotFoundObjectResult(response),
                (int)HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(response),
                (int)HttpStatusCode.Created => new CreatedAtActionResult(
                    ControllerContext.RouteData.Values["action"].ToString(),
                    ControllerContext.RouteData.Values["controller"].ToString(),
                                        //new { id = 1 }, // TODO: Hmm... need to get this from the response or do something different.
                    new { id = (response as CreateTodoItemResponse).Id }, // TODO: Hmm... not very clean.  Look at this later.
                    response),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };

        private static StatusCodeResult CreateInternalServerError() => new StatusCodeResult((int)HttpStatusCode.InternalServerError);
    }
}
