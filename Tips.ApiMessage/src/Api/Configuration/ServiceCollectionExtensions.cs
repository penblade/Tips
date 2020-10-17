using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.Api.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            Pipeline.Configuration.DependencyRegistrar.Register(services);
            Middleware.Configuration.DependencyRegistrar.Register(services, configuration);
            Security.Configuration.DependencyRegistrar.Register(services, configuration);
            Rules.Configuration.DependencyRegistrar.Register(services);
            TodoItems.Configuration.DependencyRegistrar.Register(services);
        }
    }
}
