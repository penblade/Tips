using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tips.Security
{
    public class ApiKeyHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApiKeyRepository _apiKeyRepository;

        public ApiKeyHandlerMiddleware(RequestDelegate next, IApiKeyRepository apiKeyRepository)
        {
            _next = next;
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var apiKeysFromHeaders = _apiKeyRepository.GetApiKeysFromHeaders(context).ToList();

            if (ApiKeyFound(apiKeysFromHeaders))
            {
                await _next.Invoke(context);
            }
            else if (ApiKeyNotFound(apiKeysFromHeaders))
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            }
            else if (ApiKeyHasDuplicates(apiKeysFromHeaders))
            {
                throw new InvalidOperationException(CreateDuplicateApiKeyError(apiKeysFromHeaders));
            }
        }

        private static bool ApiKeyHasDuplicates(ICollection apiKeysFromHeaders) => apiKeysFromHeaders.Count > 1;
        private static bool ApiKeyNotFound(ICollection apiKeysFromHeaders) => apiKeysFromHeaders.Count < 1;
        private static bool ApiKeyFound(ICollection apiKeysFromHeaders) => apiKeysFromHeaders.Count == 1;

        private static string CreateDuplicateApiKeyError(IEnumerable<ApiKey> apiKeysFromHeaders)
        {
            return "The following ApiKey's share the same id.  Fix immediately!!! | " +
                   $"{string.Join(", ", apiKeysFromHeaders.Select(apiKey => apiKey.Owner))}";
        }
    }
}