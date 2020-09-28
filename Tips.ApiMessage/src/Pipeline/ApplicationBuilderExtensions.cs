using Microsoft.AspNetCore.Builder;

namespace Tips.Pipeline
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
