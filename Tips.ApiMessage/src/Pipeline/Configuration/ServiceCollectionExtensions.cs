using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.Pipeline.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDependenciesForPipeline(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IPipelineBehavior), typeof(LoggingBehavior));

            var config = new ExceptionHandlerMiddlewareConfiguration();
            configuration.Bind(nameof(ExceptionHandlerMiddlewareConfiguration), config);
            services.AddSingleton(config);
        }
    }
}
