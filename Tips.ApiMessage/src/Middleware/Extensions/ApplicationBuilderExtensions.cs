using Microsoft.AspNetCore.Builder;
using Tips.Middleware.ExceptionHandling;
using Tips.Middleware.Security;

namespace Tips.Middleware.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandlerMiddleware>();
        public static IApplicationBuilder UseApiKeyHandlerMiddleware(this IApplicationBuilder builder) => builder.UseMiddleware<ApiKeyHandlerMiddleware>();
    }
}
