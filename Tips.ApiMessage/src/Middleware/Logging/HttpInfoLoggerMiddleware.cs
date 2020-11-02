﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tips.Middleware.Extensions;
using Tips.Pipeline.Extensions;
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

        // RemoteIpAddress = ::1 is localhost
        // https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core

        public async Task InvokeAsync(HttpContext context)
        {
            LogRequest(context);
            await _next(context);
            LogResponse(context);
        }

        private void LogRequest(HttpContext context)
        {
            var apiKeyOwner = _apiKeyRepository.GetApiKeyFromHeaders(context)?.Owner;

            const string scope = "Processing Request";
            using (_logger.BeginScopeWithApiTraceParentId())
            using (_logger.BeginScopeWithApiTraceParentStateString())
            using (_logger.BeginScopeWithApiTraceId())
            using (_logger.BeginScopeWithApiTraceStateString(scope))
            using (_logger.BeginScopeWithApiScope(scope))
            {
                _logger.LogRequest(context, apiKeyOwner);
            }
        }

        private void LogResponse(HttpContext context)
        {
            const string scope = "Returning Response";
            using (_logger.BeginScopeWithApiTraceParentId())
            using (_logger.BeginScopeWithApiTraceParentStateString())
            using (_logger.BeginScopeWithApiTraceId())
            using (_logger.BeginScopeWithApiTraceStateString(scope))
            using (_logger.BeginScopeWithApiScope(scope))
            {
                _logger.LogResponse(context);
            }
        }
    }
}
