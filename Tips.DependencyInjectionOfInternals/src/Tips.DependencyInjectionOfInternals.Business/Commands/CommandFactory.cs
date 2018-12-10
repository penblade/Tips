using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business.Commands
{
    internal class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<ICommand> Create(CommandType commandType)
        {
            // Services will returned in the order they were registered in the Startup.
            var commands = _serviceProvider.GetServices<ICommand>();
            commands = commandType == CommandType.All
                ? commands
                : commands.Where(x => x.GetType().Name.Equals(commandType.ToString()));
            return commands;
        }
    }
}
