using Microsoft.AspNetCore.Builder;
using Tips.Middleware.ExceptionHandling;

namespace Tips.Middleware.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
