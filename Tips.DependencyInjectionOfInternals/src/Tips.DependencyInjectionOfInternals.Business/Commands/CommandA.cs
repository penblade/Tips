using Tips.DependencyInjectionOfInternals.Business.Configuration;
using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business.Commands
{
    internal class CommandA : ICommand
    {
        private readonly BusinessConfiguration _businessConfiguration;

        public CommandA(BusinessConfiguration businessConfiguration)
        {
            _businessConfiguration = businessConfiguration;
        }

        public string Process(ProcessRequest request)
        {
            // Do something
            var connectionString = _businessConfiguration.ConnectionString;

            // Return a message to demonstrate when the command was processed.
            return $@"CommandA was processed.  ConnectionString: {connectionString}";
        }
    }
}
