using Microsoft.AspNetCore.Builder;

namespace Tips.Security.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiKeyHandlerMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ApiKeyHandlerMiddleware>();
    }
}
