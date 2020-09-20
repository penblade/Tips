using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    internal class UpdateTodoItemRequestHandler : IRequestHandler<UpdateTodoItemRequest, Response>
    {
        private readonly IRulesEngine _rulesEngine;
        private readonly IRulesFactory<Request<TodoItem>, Response<TodoItem>> _saveRulesFactory;
        private readonly IRulesFactory<UpdateTodoItemRequest, Response<TodoItem>> _updateRulesFactory;
        private readonly IUpdateTodoItemRepository _updateTodoItemRepository;

        public UpdateTodoItemRequestHandler(IRulesEngine rulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItem>> saveRulesFactory,
            IRulesFactory<UpdateTodoItemRequest, Response<TodoItem>> updateRulesFactory,
            IUpdateTodoItemRepository updateTodoItemRepository)
        {
            _rulesEngine = rulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _updateRulesFactory = updateRulesFactory;
            _updateTodoItemRepository = updateTodoItemRepository;
        }

        public async Task<Response> Handle(UpdateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItem>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            if (ProcessRules(request, response, _updateRulesFactory.Create().ToList())) return response;
            if (ProcessRules(request, response, _saveRulesFactory.Create().ToList())) return response;

            // Command.  Save the data.
            return await _updateTodoItemRepository.Save(response, cancellationToken);
        }

        private bool ProcessRules<TRequest>(TRequest request, Response<TodoItem> response, IReadOnlyCollection<BaseRule<TRequest, Response<TodoItem>>> rules)
        {
            _rulesEngine.Process(request, response, rules);
            var rulesFailed = rules.Any(rule => rule.Failed);
            if (rulesFailed && response.IsStatusNotSet()) response.SetStatusToBadRequest();
            return rulesFailed;
        }
    }
}
