using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    internal class ServiceLifetimeWrapper
    {
        public const string NotSet = "NotSet";
        
        // Transient lifetime services are created each time they're requested. This lifetime works best for lightweight, stateless services.
        public const string Transient = "Transient";
        
        // Scoped lifetime services are created once per request.
        public const string Scoped = "Scoped";

        // Singleton lifetime services are created the first time they're requested.  Every subsequent request uses the same instance.
        public const string Singleton = "Singleton";

        private static readonly IEnumerable<ServiceLifetimeWrapper> ServiceLifetimes =
            typeof(ServiceLifetimeWrapper)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
            .Select(fieldInfo => new ServiceLifetimeWrapper(fieldInfo.Name))
            .ToList();

        private ServiceLifetimeWrapper(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public static ServiceLifetimeWrapper FromName(string name)
        {
            var serviceLifetime = ServiceLifetimes.SingleOrDefault(x => x.Name == name);
            if (serviceLifetime == null) throw new ArgumentException($"Could not get a ServiceLifetimeWrapper from name: {name}");
            
            return serviceLifetime;
        } 
    }
}
