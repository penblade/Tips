using Microsoft.Extensions.DependencyInjection;
using Tips.Pipeline.Logging;

namespace Tips.Pipeline.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior), typeof(LoggingBehavior));
        }
    }
}
