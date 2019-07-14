using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business.Commands
{
    internal class CommandB : ICommand
    {
        public string Process(ProcessRequest request)
        {
            // Do something
            // Return a message to demonstrate when the command was processed.
            return "CommandB was processed.";
        }
    }
}
