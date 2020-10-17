using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tips.Security
{
    public class ApiKeyHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiKeyConfiguration _apiKeyConfiguration;

        internal  ApiKeyHandlerMiddleware(RequestDelegate next, ApiKeyConfiguration apiKeyConfiguration)
        {
            _next = next;
            _apiKeyConfiguration = apiKeyConfiguration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!IsValidApiKey(context))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        private bool IsValidApiKey(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(_apiKeyConfiguration.ApiHeader, out var apiKeyInHeaders)) return false;

            return _apiKeyConfiguration.ApiKeys.Any(apiKey =>
                string.Equals(apiKey.Key, apiKeyInHeaders.ToString(), StringComparison.OrdinalIgnoreCase));
        }
    }
}