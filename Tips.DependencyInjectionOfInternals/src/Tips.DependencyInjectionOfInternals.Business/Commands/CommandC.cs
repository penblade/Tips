using Tips.DependencyInjectionOfInternals.Business.Configuration;
using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business.Commands
{
    internal class CommandC : ICommand
    {
        private readonly BusinessConfiguration _businessConfiguration;

        public CommandC(BusinessConfiguration businessConfiguration)
        {
            _businessConfiguration = businessConfiguration;
        }

        public string Process(ProcessRequest request)
        {
            // Do something
            var documentPath = _businessConfiguration.DocumentPath;

            // Return a message to demonstrate when the command was processed.
            return $@"CommandC was processed.  DocumentPath: {documentPath}";
        }
    }
}
