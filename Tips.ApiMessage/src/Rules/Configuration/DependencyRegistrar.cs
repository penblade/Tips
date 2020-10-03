using Microsoft.Extensions.DependencyInjection;

namespace Tips.Rules.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped(typeof(IRulesEngine), typeof(RulesEngine));
            services.AddScoped(typeof(IRulesFactory<,>), typeof(RulesFactory<,>));
        }
    }
}
