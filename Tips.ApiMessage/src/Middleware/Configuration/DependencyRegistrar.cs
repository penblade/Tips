using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.Middleware.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            var config = new ExceptionHandlerMiddlewareConfiguration();
            configuration.Bind(nameof(ExceptionHandlerMiddlewareConfiguration), config);
            services.AddSingleton(config);
        }
    }
}
