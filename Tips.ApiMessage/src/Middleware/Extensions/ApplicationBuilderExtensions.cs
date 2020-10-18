using Microsoft.AspNetCore.Builder;
using Tips.Middleware.ExceptionHandling;
using Tips.Middleware.Logging;

namespace Tips.Middleware.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandlerMiddleware>();
        public static IApplicationBuilder ConfigureHttpInfoLogger(this IApplicationBuilder builder) => builder.UseMiddleware<HttpInfoLoggerMiddleware>();
    }
}
