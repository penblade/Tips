using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    internal static class DependencyRegistrar
    {
        internal static void RegisterDependencyConfiguration(IServiceCollection services, DependencyConfiguration configuration)
        {
            // Services are returned in the order they were registered in the Startup.
            foreach (var dependency in configuration.Dependencies)
            {
                var (serviceLifetime, serviceType, implementationType) = ParseDependency(configuration.Namespace, dependency);
                RegisterDependencyByServiceLifeTime(services, serviceLifetime, serviceType, implementationType);
            }
        }

        private static (ServiceLifetime serviceLifetime, Type serviceType, Type implementationType) ParseDependency(string configurationNamespace, Dependency dependency)
        {
            var serviceType = ParseType(BuildTypeName(configurationNamespace, dependency.Namespace, dependency.ServiceType));
            var implementationType = ParseType(BuildTypeName(configurationNamespace, dependency.Namespace, dependency.ImplementationType));

            return (Enum.Parse<ServiceLifetime>(dependency.ServiceLifetime), serviceType, implementationType);
        }

        private static Type ParseType(string typeName) => Type.GetType(typeName);

        private static string BuildTypeName(string namespaceStart, string namespaceEnd, string typeName) => $"{namespaceStart}.{namespaceEnd}.{typeName}";

        private static void RegisterDependencyByServiceLifeTime(IServiceCollection services, ServiceLifetime serviceLifetime,
            Type serviceType, Type implementationType)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, "Configuration Error: Dependency service lifetime does not exist.");
            }
        }
    }
}
