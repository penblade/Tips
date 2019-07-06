namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    internal class Dependency
    {
        public string Namespace { get; set; }
        public string ServiceLifetime { get; set; }
        public string ServiceType { get; set; }
        public string ImplementationType { get; set; }
    }
}
