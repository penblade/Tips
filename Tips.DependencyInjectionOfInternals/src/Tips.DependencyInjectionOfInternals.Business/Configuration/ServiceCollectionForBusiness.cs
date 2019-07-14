using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    public class ServiceCollectionForBusiness : IServiceCollectionForBusiness
    {
        private IConfiguration _appSettingsConfiguration;
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
            var businessConfiguration = RegisterBusinessConfiguration(services);
            var dependencyConfig = RegisterDependencyConfiguration(services, businessConfiguration?.IocFiles);
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

        private BusinessConfiguration RegisterBusinessConfiguration(IServiceCollection services)
        {
            // Bind the BusinessConfiguration section from the appsettings.json.
            const string sectionName = nameof(BusinessConfiguration);
            var businessConfiguration = CreateConfigurationByType<BusinessConfiguration>();
            _appSettingsConfiguration.Bind(sectionName, businessConfiguration);
            services.AddSingleton(businessConfiguration);
            return businessConfiguration;
        }

        private DependencyConfiguration RegisterDependencyConfiguration(IServiceCollection services,
            IEnumerable<string> iocFiles)
        {
            // Bind the json files.  Update the app settings configuration after adding json files.
            _appSettingsConfiguration = AddJsonFiles(_configurationBuilder, iocFiles);
            var dependencyConfig = CreateConfigurationByType<DependencyConfiguration>();
            _appSettingsConfiguration.Bind(dependencyConfig);
            services.AddSingleton(dependencyConfig);
            return dependencyConfig;
        }

        private static T CreateConfigurationByType<T>() => (T)Activator.CreateInstance(typeof(T));

        private static IConfiguration AddJsonFiles(IConfigurationBuilder configurationBuilder, IEnumerable<string> jsonFiles)
        {
            if (jsonFiles == null) throw new ArgumentException("No Json files were found.");

            foreach (var file in jsonFiles)
            {
                configurationBuilder.AddJsonFile(file, false, true);
            }
            return configurationBuilder.Build();
        }
    }
}
