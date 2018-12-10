using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.DependencyInjectionOfInternals.Business.Configuration
{
    public interface IServiceCollectionForBusiness
    {
        void RegisterDependencies(IConfiguration configuration, IServiceCollection services);
    }
}