using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Tips.Security
{
    public interface IApiKeyRepository
    {
        public ApiKey GetApiKeyFromHeaders(HttpContext context);
        public IEnumerable<ApiKey> GetApiKeysFromHeaders(HttpContext context);
    }
}
