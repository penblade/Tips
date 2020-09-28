using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tips.Pipeline.Configuration;
using Tips.Rules.Configuration;
using Tips.TodoItems.Configuration;

namespace Tips.ApiMessage.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDependenciesForRules();
            services.RegisterDependenciesForPipeline(configuration);
            services.RegisterDependenciesForTodoItems();
        }
    }
}
