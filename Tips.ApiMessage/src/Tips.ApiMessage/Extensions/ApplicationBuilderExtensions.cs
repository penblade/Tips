﻿using Microsoft.AspNetCore.Builder;
using Tips.ApiMessage.Pipeline;

namespace Tips.ApiMessage.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app) => app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}