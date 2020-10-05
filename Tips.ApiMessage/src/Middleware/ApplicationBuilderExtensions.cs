using Microsoft.AspNetCore.Builder;
using Tips.Middleware.Security;

namespace Tips.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandlerMiddleware>();
        public static IApplicationBuilder UseApiKeyHandlerMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ApiKeyHandlerMiddleware>();
    }
}
