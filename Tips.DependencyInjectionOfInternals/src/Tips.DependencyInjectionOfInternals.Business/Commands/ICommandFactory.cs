using System.Collections.Generic;
using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business.Commands
{
    internal interface ICommandFactory
    {
        IEnumerable<ICommand> Create(CommandType commandType);
    }
}