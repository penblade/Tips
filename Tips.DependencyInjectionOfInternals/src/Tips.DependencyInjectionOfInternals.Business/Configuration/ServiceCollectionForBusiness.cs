using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tips.DependencyInjectionOfInternals.Business.Commands;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    public class ServiceCollectionForBusiness : IServiceCollectionForBusiness
    {
        public void RegisterDependencies(IConfiguration configuration, IServiceCollection services)
        {
            // Bind the configuration to 
            var config = new BusinessConfiguration();
            configuration.Bind(nameof(BusinessConfiguration), config);
            services.AddSingleton(config);

            // Setup relationship between public interfaces and internal classes
            services.AddScoped<IBusinessService, BusinessService>();

            // Setup relationship between internal interfaces and internal classes
            services.AddScoped<ICommandFactory, CommandFactory>();

            // Services will returned in the order they were registered in the Startup.
            services.AddScoped<ICommand, CommandB>();
            services.AddScoped<ICommand, CommandA>();
            services.AddScoped<ICommand, CommandC>();
        }
    }
}
