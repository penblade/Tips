using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Tips.Security
{
    internal class ApiKeyRepository : IApiKeyRepository
    {
        private readonly ApiKeyConfiguration _apiKeyConfiguration;

        public ApiKeyRepository(ApiKeyConfiguration apiKeyConfiguration) => _apiKeyConfiguration = apiKeyConfiguration;

        public ApiKey GetApiKeyFromHeaders(HttpContext context) => GetApiKeysFromHeaders(context)?.FirstOrDefault();

        public IEnumerable<ApiKey> GetApiKeysFromHeaders(HttpContext context)
        {
            if (_apiKeyConfiguration?.ApiKeys == null || !TryGetApiKeyIdFromHeaders(context, out var apiKeyInHeaders)) return new List<ApiKey>();

            return _apiKeyConfiguration.ApiKeys?.Where(apiKey =>
                string.Equals(apiKey.Key, apiKeyInHeaders, StringComparison.OrdinalIgnoreCase));
        }

        private bool TryGetApiKeyIdFromHeaders(HttpContext context, out string apiKeyId)
        {
            var result = context.Request.Headers.TryGetValue(_apiKeyConfiguration.ApiHeader, out var apiKeyIdInHeaders);
            apiKeyId = apiKeyIdInHeaders;
            return result;
        }
    }
}
