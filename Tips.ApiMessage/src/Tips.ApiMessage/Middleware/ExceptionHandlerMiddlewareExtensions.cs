using Microsoft.AspNetCore.Builder;

namespace Tips.ApiMessage.Middleware
{
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
