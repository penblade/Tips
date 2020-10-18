using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Tips.Security
{
    public class ApiKeyHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiKeyHandlerMiddleware> _logger;
        private readonly IApiKeyRepository _apiKeyRepository;

        public ApiKeyHandlerMiddleware(RequestDelegate next, ILogger<ApiKeyHandlerMiddleware> logger, IApiKeyRepository apiKeyRepository)
        {
            _next = next;
            _logger = logger;
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task Invoke(HttpContext context)
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
                _logger.LogError(CreateDuplicateApiKeyError(apiKeysFromHeaders));
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            }
        }

        private static bool ApiKeyHasDuplicates(ICollection apiKeysFromHeaders) => apiKeysFromHeaders.Count > 1;
        private static bool ApiKeyNotFound(ICollection apiKeysFromHeaders) => apiKeysFromHeaders.Count < 1;
        private static bool ApiKeyFound(ICollection apiKeysFromHeaders) => apiKeysFromHeaders.Count == 1;

        private static string CreateDuplicateApiKeyError(IEnumerable<ApiKey> apiKeysFromHeaders)
        {
            return "The following ApiKey's share the same api key.  Fix immediately!!! | " +
                   $"{string.Join(", ", apiKeysFromHeaders.Select(apiKey => apiKey.Owner))}";
        }
    }
}