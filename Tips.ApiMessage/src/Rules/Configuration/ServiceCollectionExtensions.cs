using Microsoft.Extensions.DependencyInjection;

namespace Tips.Rules.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDependenciesForRules(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRulesEngine), typeof(RulesEngine));
        }
    }
}
