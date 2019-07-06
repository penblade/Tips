using Microsoft.Extensions.DependencyInjection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    public interface IServiceCollectionForBusiness
    {
        void RegisterDependencies(IServiceCollection services);
    }
}