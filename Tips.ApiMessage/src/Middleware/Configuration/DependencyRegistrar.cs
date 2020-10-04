using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.Middleware.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            // Middleware must be injected as a singleton.
            var config = new ProblemDetailConfiguration();
            configuration.Bind(nameof(ProblemDetailConfiguration), config);
            services.AddSingleton(config);

            // This is a dependency within the Middleware class, so it too must be injected as a singleton.
            services.AddSingleton(typeof(IProblemDetailFactory), typeof(ProblemDetailFactory));
        }
    }
}
