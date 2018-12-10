using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business.Commands
{
    internal interface ICommand
    {
        string Process(ProcessRequest request);
    }
}