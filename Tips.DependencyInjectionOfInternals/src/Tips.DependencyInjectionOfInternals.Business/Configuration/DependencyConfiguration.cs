using System.Collections.Generic;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    internal class DependencyConfiguration
    {
        public string Namespace { get; set; }
        public IEnumerable<Dependency> Dependencies { get; set; }
    }
}
