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
            RegisterDependencyConfiguration(services, dependencyConfig);
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

        private static void RegisterDependencyConfiguration(IServiceCollection services, DependencyConfiguration configuration)
        {
            // Services are returned in the order they were registered in the Startup.
            foreach (var dependency in configuration.Dependencies)
            {
                var (serviceLifetime, serviceType, implementationType) = ParseDependency(configuration, dependency);
                RegisterDependencyByServiceLifeTime(services, serviceLifetime, serviceType, implementationType);
            }
        }

        private static (ServiceLifetime serviceLifetime, Type serviceType, Type implementationType) ParseDependency(DependencyConfiguration configuration, Dependency dependency)
        {
            var serviceLifetime = ServiceLifetime.FromName(dependency?.ServiceLifetime);

            var serviceType = ParseType(BuildTypeName(configuration.Namespace, dependency.Namespace, dependency.ServiceType));
            var implementationType = ParseType(BuildTypeName(configuration.Namespace, dependency.Namespace, dependency.ImplementationType));

            return (serviceLifetime, serviceType, implementationType);
        }

        private static void RegisterDependencyByServiceLifeTime(IServiceCollection services, ServiceLifetime serviceLifetime,
            Type serviceType, Type implementationType)
        {
            if (serviceLifetime == ServiceLifetime.Scoped) services.AddScoped(serviceType, implementationType);
            else if (serviceLifetime == ServiceLifetime.Transient) services.AddTransient(serviceType, implementationType);
            else if (serviceLifetime == ServiceLifetime.Singleton) services.AddSingleton(serviceType, implementationType);
            else if (serviceLifetime == ServiceLifetime.NotSet) throw new ArgumentException("Configuration Error: Dependency service lifetime was not set.");
            else throw new ArgumentOutOfRangeException(nameof(serviceLifetime), "Configuration Error: Dependency service lifetime does not exist.");
        }

        private static Type ParseType(string typeName) => Type.GetType(typeName);

        private static string BuildTypeName(string namespaceStart, string namespaceEnd, string typeName) => $"{namespaceStart}.{namespaceEnd}.{typeName}";
    }
}
