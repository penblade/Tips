using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    internal class DependencyRegistrar
    {
        internal static void RegisterDependencyConfiguration(IServiceCollection services, DependencyConfiguration configuration)
        {
            // Services are returned in the order they were registered in the Startup.
            foreach (var dependency in configuration.Dependencies)
            {
                var (serviceLifetime, serviceType, implementationType) = ParseDependency(configuration, dependency);
                RegisterDependencyByServiceLifeTime(services, serviceLifetime, serviceType, implementationType);
            }
        }

        private static (ServiceLifetimeWrapper serviceLifetime, Type serviceType, Type implementationType) ParseDependency(DependencyConfiguration configuration, Dependency dependency)
        {
            var serviceType = ParseType(BuildTypeName(configuration.Namespace, dependency.Namespace, dependency.ServiceType));
            var implementationType = ParseType(BuildTypeName(configuration.Namespace, dependency.Namespace, dependency.ImplementationType));

            return (ServiceLifetimeWrapper.FromName(dependency.ServiceLifetime), serviceType, implementationType);
        }

        private static void RegisterDependencyByServiceLifeTime(IServiceCollection services, ServiceLifetimeWrapper serviceLifetimeWrapper,
            Type serviceType, Type implementationType)
        {
            switch (serviceLifetimeWrapper.Name)
            {
                case ServiceLifetimeWrapper.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;
                case ServiceLifetimeWrapper.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
                case ServiceLifetimeWrapper.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;
                case ServiceLifetimeWrapper.NotSet:
                    throw new ArgumentException("Configuration Error: Dependency service lifetime was not set.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetimeWrapper), "Configuration Error: Dependency service lifetime does not exist.");
            }
        }

        private static Type ParseType(string typeName) => Type.GetType(typeName);

        private static string BuildTypeName(string namespaceStart, string namespaceEnd, string typeName) => $"{namespaceStart}.{namespaceEnd}.{typeName}";
    }


}
