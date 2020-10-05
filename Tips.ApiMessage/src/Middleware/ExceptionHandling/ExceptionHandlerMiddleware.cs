using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tips.Middleware.ErrorHandling;
using Tips.Pipeline;

namespace Tips.Middleware.ExceptionHandling
{
    // Inspiration: https://code-maze.com/global-error-handling-aspnetcore/#builtinmiddleware
    internal class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IProblemDetailsFactory _problemDetailsFactory;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IProblemDetailsFactory problemDetailsFactory)
        {
            _next = next;
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var problemDetails = _problemDetailsFactory.InternalServerError();
                LogError(problemDetails, exception);
                await WriteResponseAsync(context, problemDetails);
            }
        }

        private void LogError(ProblemDetails problemDetails, Exception exception)
        {
            using var scope = _logger.BeginScope(nameof(ExceptionHandlerMiddleware));
            _logger.LogError(CreateLogMessageForProblemDetails(JsonSerializer.Serialize(problemDetails)), problemDetails);
            _logger.LogError(exception, CreateLogMessageForUncaughtException(), exception);
        }

        private static async Task WriteResponseAsync(HttpContext context, ProblemDetails problemDetails)
        {
            // Add same headers returned by the built-in exception handler.
            context.Response.Headers["Cache-Control"] = "no-cache";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "-1";
            context.Response.StatusCode = problemDetails.Status ?? 0;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }

        private static string CreateLogMessageForUncaughtException() => @$"TraceId: {Tracking.TraceId} | UncaughtException{Environment.NewLine}An unhandled exception has occurred while executing the request.{Environment.NewLine}";
        private static string CreateLogMessageForProblemDetails(string problemDetails) => @$"TraceId: {Tracking.TraceId} | ProblemDetails: {LogFormatter.FormatForLogging(problemDetails)}";
    }
}
