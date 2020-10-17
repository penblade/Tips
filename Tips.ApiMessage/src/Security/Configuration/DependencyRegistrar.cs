using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.Security.Configuration
{
    public static class DependencyRegistrar
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
            var apiKeyConfiguration = new ApiKeyConfiguration();
            configuration.Bind(nameof(ApiKeyConfiguration), apiKeyConfiguration);
            services.AddSingleton(apiKeyConfiguration);
        }
    }
}
