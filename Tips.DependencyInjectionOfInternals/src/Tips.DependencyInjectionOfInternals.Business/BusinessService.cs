using Tips.DependencyInjectionOfInternals.Business.Commands;
using Tips.DependencyInjectionOfInternals.Business.Models;

namespace Tips.DependencyInjectionOfInternals.Business
{
    internal class BusinessService : IBusinessService
    {
        private readonly ICommandFactory _commandFactory;

        public BusinessService(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public ProcessResponse Process(ProcessRequest request)
        {
            var response = new ProcessResponse();
            foreach (var command in _commandFactory.Create(request.CommandType))
            {
                response.Messages.Add(command.Process(request));
            }

            return response;
        }
    }
}
