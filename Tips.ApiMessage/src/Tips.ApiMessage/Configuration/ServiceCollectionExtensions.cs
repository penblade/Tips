using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.ApiMessage.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            Rules.Configuration.DependencyRegistrar.Register(services);
            Pipeline.Configuration.DependencyRegistrar.Register(services, configuration);
            TodoItems.Configuration.DependencyRegistrar.Register(services);
        }
    }
}
