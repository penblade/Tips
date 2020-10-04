using Microsoft.AspNetCore.Builder;

namespace Tips.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
