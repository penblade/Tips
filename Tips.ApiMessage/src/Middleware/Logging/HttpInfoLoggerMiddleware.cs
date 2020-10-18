using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Tips.Pipeline;
using Tips.Security;

namespace Tips.Middleware.Logging
{
    internal class HttpInfoLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpInfoLoggerMiddleware> _logger;
        private readonly IApiKeyRepository _apiKeyRepository;

        public HttpInfoLoggerMiddleware(RequestDelegate next, ILogger<HttpInfoLoggerMiddleware> logger, IApiKeyRepository apiKeyRepository)
        {
            _next = next;
            _logger = logger;
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                _logger.LogInformation(CreateHttpLogMessage(context));
            }
        }

        private string CreateHttpLogMessage(HttpContext context)
        {
            // RemoteIpAddress = ::1 is localhost
            // https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core

            var apiKeyOwner = _apiKeyRepository.GetApiKeyFromHeaders(context)?.Owner;
            var httpStatusCodeName = GetHttpStatusCodeName(context);

            var message =
                $"{CreateClientInfo(context, apiKeyOwner)}{Environment.NewLine}" +
                $"{CreateRequestInfo(context)} => {CreateResponseInfo(context, httpStatusCodeName)}";
            
            return CreateLogMessage(message);
        }

        private static string CreateClientInfo(HttpContext context, string owner) =>
            $"IP: {context.Connection.RemoteIpAddress} | ApiKey.Owner: {owner}";

        private static string CreateRequestInfo(HttpContext context) =>
            $"{context.Request?.Method} {context.Request?.GetDisplayUrl()}";
        
        private static string CreateResponseInfo(HttpContext context, string httpStatusCodeName) =>
            $"{context.Request?.Protocol} {context.Response?.StatusCode} {httpStatusCodeName}";

        private static string GetHttpStatusCodeName(HttpContext context) =>
            context.Response?.StatusCode != null ? Enum.GetName(typeof(HttpStatusCode), context.Response?.StatusCode) : "";

        private static string CreateLogMessage(string response) => @$"TraceId: {Tracking.TraceId} | {LogFormatter.FormatForLogging(response)}";
    }
}
