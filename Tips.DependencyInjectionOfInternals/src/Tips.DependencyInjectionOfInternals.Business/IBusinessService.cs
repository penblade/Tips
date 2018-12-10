using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business
{
    public interface IBusinessService
    {
        ProcessResponse Process(ProcessRequest request);
    }
}