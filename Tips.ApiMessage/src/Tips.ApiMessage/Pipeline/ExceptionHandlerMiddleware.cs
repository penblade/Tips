using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tips.ApiMessage.Configuration;

namespace Tips.ApiMessage.Pipeline
{
    // Inspiration: https://code-maze.com/global-error-handling-aspnetcore/#builtinmiddleware
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly AppConfiguration _configuration;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, AppConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var problemDetails = CreateProblemDetails(TraceId, _configuration.UrnName);

                using var scope = _logger.BeginScope(nameof(ExceptionHandlerMiddleware));
                _logger.LogError(CreateLogMessageForProblemDetails(TraceId, JsonSerializer.Serialize(problemDetails)), problemDetails);
                _logger.LogError(exception, CreateLogMessageForUncaughtException(TraceId), exception);

                // Add same headers returned by the built-in exception handler.
                context.Response.Headers["Cache-Control"] = "no-cache";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["Expires"] = "-1";
                context.Response.StatusCode = problemDetails.Status ?? 0;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            }
        }

        private static ProblemDetails CreateProblemDetails(string traceId, string urnName)
        {
            const string uncaughtExceptionId = "D1537B75-D85A-48CF-8A02-DF6C614C3198";

            // ProblemDetails implements the RF7807 standards.
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = (int) HttpStatusCode.InternalServerError,
                Detail = "Internal Server Error",
                Instance = $"urn:{urnName}:error:{uncaughtExceptionId}"
            };

            problemDetails.Extensions["traceId"] = traceId;
            return problemDetails;
        }

        private static string TraceId => Activity.Current?.Id;
        private static string CreateLogMessageForUncaughtException(string traceId) => @$"TraceId: {traceId} | UncaughtException{Environment.NewLine}An unhandled exception has occurred while executing the request.{Environment.NewLine}";
        private static string CreateLogMessageForProblemDetails(string traceId, string problemDetails) => @$"TraceId: {traceId} | ProblemDetails: {LogFormatter.FormatForLogging(problemDetails)}";
    }
}
