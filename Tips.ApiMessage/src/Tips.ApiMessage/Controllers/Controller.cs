﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tips.ApiMessage.Handlers;

namespace Tips.ApiMessage.Controllers
{
    public class Controller : ControllerBase
    {
        public delegate Task<TResponse> HandleDelegate<in TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);

        protected async Task<IActionResult> Handle<TRequest, TResponse>(HandleDelegate<TRequest, TResponse> method, TRequest request, CancellationToken cancellationToken) where TResponse : Response
        {
            try
            {
                // TODO: Log call to service.
                // TODO: Log request if provided.
                var response = await method(request, cancellationToken);
                var actionResult = CreateActionResult(response);

                // TODO: Log actionResult
                return actionResult;
            }
            catch (Exception ex)
            {
                // TODO: Log exception
                return CreateInternalServerError();
            }
        }

        private static IActionResult CreateActionResult<TResponse>(TResponse response) where TResponse : Response =>
            response?.ApiMessage?.Status switch
            {
                null => throw new Exception("HttpStatusCode was not set."),
                (int)HttpStatusCode.OK => new OkObjectResult(response),
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
                (int)HttpStatusCode.NotFound => new NotFoundObjectResult(response),
                (int)HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(response),
                _ => throw new Exception($"HttpStatusCode {response.ApiMessage.Status} was not handled.")
            };

        private static StatusCodeResult CreateInternalServerError() => new StatusCodeResult((int)HttpStatusCode.InternalServerError);
    }
}
