using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tips.Middleware.ErrorHandling;

namespace Tips.Middleware.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            // Middleware must be injected as a singleton.
            var problemDetailsConfiguration = new ProblemDetailsConfiguration();
            configuration.Bind(nameof(ProblemDetailsConfiguration), problemDetailsConfiguration);
            services.AddSingleton(problemDetailsConfiguration);

            // This is a dependency within the Middleware class, so it too must be injected as a singleton.
            services.AddSingleton(typeof(IProblemDetailsFactory), typeof(ProblemDetailsFactory));
        }
    }
}
