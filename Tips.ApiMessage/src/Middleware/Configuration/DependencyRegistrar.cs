using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tips.Middleware.Security;

namespace Tips.Middleware.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            var apiKeyConfiguration = new ApiKeyConfiguration();
            configuration.Bind(nameof(ApiKeyConfiguration), apiKeyConfiguration);
            services.AddSingleton(apiKeyConfiguration);

            // Middleware must be injected as a singleton.
            var problemDetailsConfiguration = new ProblemDetailsConfiguration();
            configuration.Bind(nameof(ProblemDetailsConfiguration), problemDetailsConfiguration);
            services.AddSingleton(problemDetailsConfiguration);

            // This is a dependency within the Middleware class, so it too must be injected as a singleton.
            services.AddSingleton(typeof(IProblemDetailsFactory), typeof(ProblemDetailsFactory));
        }
    }
}
