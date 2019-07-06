using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    internal static class AssemblyTypes
    {
        public static IEnumerable<(Type serviceType, Type implementationType)> GetByDefaultConvention()
        {
            var results = new List<(Type, Type)>();

            var assembly = Assembly.GetExecutingAssembly();
            var assemblyClasses = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract).ToList();
            var assemblyInterfaces = assembly.GetTypes().Where(x => x.IsInterface).ToList();

            // If there are no interfaces or classes, then there is nothing to register.
            if (!assemblyInterfaces.Any() || !assemblyClasses.Any()) return results;

            // Do this by interface to ignore all of the POCOs.
            foreach (var assemblyInterface in assemblyInterfaces)
            {
                var assemblyClass = assemblyClasses.SingleOrDefault(
                    x => x.Name == assemblyInterface.Name.TrimStart('I') &&
                         x.GetInterfaces().FirstOrDefault(y => y.FullName == assemblyInterface.FullName) != null);

                // If a class is not found that implements the default conventions, then there is nothing to register.
                if (assemblyClass == null) continue;

                results.Add((assemblyInterface, assemblyClass));
            }

            return results;
        }
    }
}
