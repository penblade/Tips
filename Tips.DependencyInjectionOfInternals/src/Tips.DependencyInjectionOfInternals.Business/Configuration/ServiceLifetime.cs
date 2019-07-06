using System;
using System.Collections.Generic;
using System.Linq;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    internal class ServiceLifetime
    {
        private static readonly List<ServiceLifetime> Items = new List<ServiceLifetime>();

        public static ServiceLifetime NotSet { get; } = new ServiceLifetime(0, "NotSet");
        
        // Transient lifetime services are created each time they're requested. This lifetime works best for lightweight, stateless services.
        public static ServiceLifetime Transient { get; } = new ServiceLifetime(1, "Transient");
        
        // Scoped lifetime services are created once per request.
        public static ServiceLifetime Scoped { get; } = new ServiceLifetime(2, "Scoped");

        // Singleton lifetime services are created the first time they're requested.  Every subsequent request uses the same instance.
        public static ServiceLifetime Singleton { get; } = new ServiceLifetime(3, "Singleton");

        private ServiceLifetime(int value, string name)
        {
            Value = value;
            Name = name;
            Items.Add(this);
        }

        public string Name { get; }
        public int Value { get; }

        public static IEnumerable<ServiceLifetime> List() => Items;

        public static IDictionary<int, string> Dictionary() => Items.ToDictionary(item => item.Value, item => item.Name);

        public static ServiceLifetime FromName(string name) => Items.SingleOrDefault(item => string.Compare(item.Name, name, StringComparison.Ordinal) == 0) ?? throw new ArgumentException($"{nameof(ServiceLifetime)} does not contain the case sensitive name: '{name}'.");

        public static ServiceLifetime FromNameIgnoreCase(string name) => Items.SingleOrDefault(item => string.Compare(item.Name, name, StringComparison.OrdinalIgnoreCase) == 0) ?? throw new ArgumentException($"{nameof(ServiceLifetime)} does not contain the case insensitive name : '{name}'.");

        public static ServiceLifetime FromValue(int value) => Items.SingleOrDefault(x => x.Value == value) ?? throw new ArgumentException($"{nameof(ServiceLifetime)} does not contain value: '{value}'.");
    }
}
