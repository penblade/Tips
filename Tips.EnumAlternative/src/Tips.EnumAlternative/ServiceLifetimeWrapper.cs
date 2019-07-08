using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tips.EnumAlternative
{
    // This class is an alternative to an enum.
    // Use it when you are:
    //  - Only interested in the name
    //  - You want to take advantage of switch statements
    //  - You want to keep the content in a strongly typed container
    // Following Singleton techniques, this class is thread-safe
    //  and tested in a Parallel.For test.

    // Create all of the ServiceLifetimeWrapper with a singleton list of the enumeration.
    // Inspiration: https://csharpindepth.com/articles/singleton
    internal class ServiceLifetimeWrapper
    {
        public const string NotSet = "NotSet";

        // Transient lifetime services are created each time they're requested. This lifetime works best for lightweight, stateless services.
        public const string Transient = "Transient";

        // Scoped lifetime services are created once per request.
        public const string Scoped = "Scoped";

        // Singleton lifetime services are created the first time they're requested.  Every subsequent request uses the same instance.
        public const string Singleton = "Singleton";

        // Inspiration: https://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
        // Go through the list and only pick out the constants
        // IsLiteral determines if its value is written at compile time and not changeable
        // IsInitOnly determines if the field can be set in the body of the constructor
        // for C# a field which is readonly keyword would have both true 
        //   but a const field would have only IsLiteral equal to true
        // Don't forget to add the .ToList() or the property will be reevaluated
        //   and you will not have the same instance of the class.

        // I wanted to move this logic into a base class or helper class
        // however I wanted to keep the constructor private or at least protected.
        // I tried changing the "new" statement to Activator.CreateInstance
        // and added the first parameter as an object array, but it still
        // could not find the constructor.  If anyone wants to take
        // a turn at trying to abstract this method as a generic
        // to a base class, post your solution and let me know.  Thanks!
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