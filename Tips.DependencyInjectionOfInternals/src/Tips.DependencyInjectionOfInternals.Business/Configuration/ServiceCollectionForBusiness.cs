using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    public class ServiceCollectionForBusiness : IServiceCollectionForBusiness
    {
        private readonly IConfiguration _appSettingsConfiguration;
        private readonly IConfigurationBuilder _configurationBuilder;

        public ServiceCollectionForBusiness(IConfiguration appSettingsConfiguration,
                                            IConfigurationBuilder configurationBuilder)
        {
            _appSettingsConfiguration = appSettingsConfiguration;
            _configurationBuilder = configurationBuilder;
        }

        public void RegisterDependencies(IServiceCollection services)
        {
            RegisterByConvention(services);

            var businessConfiguration = CreateAndBindBusinessConfiguration(_appSettingsConfiguration);
            RegisterBusinessConfiguration(services, businessConfiguration);

            if (businessConfiguration?.IocFiles == null) return;

            var dependencyConfig = BindDependencyConfiguration(services, _configurationBuilder, businessConfiguration.IocFiles);
            DependencyRegistrar.RegisterDependencyConfiguration(services, dependencyConfig);
        }

        private static void RegisterByConvention(IServiceCollection services)
        {
            var assemblyTypes = AssemblyTypes.GetByDefaultConvention();

            foreach (var (serviceType, implementationType) in assemblyTypes)
            {
                services.AddScoped(serviceType, implementationType);
            }
        }

        private static BusinessConfiguration CreateAndBindBusinessConfiguration(IConfiguration configuration)
        {
            // Bind the BusinessConfiguration section from the appsettings.json.
            var businessConfiguration = new BusinessConfiguration();
            configuration.Bind(nameof(BusinessConfiguration), businessConfiguration);

            if (businessConfiguration == null) throw new ArgumentException("BusinessConfiguration was not configured.");
            return businessConfiguration;
        }

        private static void RegisterBusinessConfiguration(IServiceCollection services, BusinessConfiguration businessConfiguration)
        {
            services.AddSingleton(businessConfiguration);
        }

        private static DependencyConfiguration BindDependencyConfiguration(IServiceCollection services, IConfigurationBuilder configurationBuilder, IEnumerable<string> jsonFiles)
        {
            if (jsonFiles == null) throw new ArgumentException("No Json files were found.");

            foreach (var file in jsonFiles)
            {
                configurationBuilder.AddJsonFile(file, false, true);
            }
            var updatedConfiguration = configurationBuilder.Build();

            // Bind the json files.  If you're binding the entire appSettingsConfiguration file, don't include the section name.
            var dependencyConfig = new DependencyConfiguration();
            updatedConfiguration.Bind(dependencyConfig);

            if (dependencyConfig == null) throw new ArgumentException("Business dependencies were not configured.");

            services.AddSingleton(dependencyConfig);

            return dependencyConfig;
        }
    }
}
