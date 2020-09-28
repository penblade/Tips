using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.UpdateTodoItem
{
    public class UpdateTodoItemRequestHandler : IRequestHandler<UpdateTodoItemRequest, Response>
    {
        private readonly IRulesEngine _rulesEngine;
        private readonly IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> _saveRulesFactory;
        private readonly IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>> _updateRulesFactory;
        private readonly IUpdateTodoItemRepository _updateTodoItemRepository;

        public UpdateTodoItemRequestHandler(IRulesEngine rulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> saveRulesFactory,
            IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>> updateRulesFactory,
            IUpdateTodoItemRepository updateTodoItemRepository)
        {
            _rulesEngine = rulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _updateRulesFactory = updateRulesFactory;
            _updateTodoItemRepository = updateTodoItemRepository;
        }

        public async Task<Response> HandleAsync(UpdateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            if (await ProcessRulesAsync(request, response, _updateRulesFactory.Create().ToList())) return response;
            if (await ProcessRulesAsync(request, response, _saveRulesFactory.Create().ToList())) return response;

            // Command.  Save the data.
            await _updateTodoItemRepository.SaveAsync(response, cancellationToken);

            return response;
        }

        private async Task<bool> ProcessRulesAsync<TRequest>(TRequest request, Response<TodoItemEntity> response, IReadOnlyCollection<BaseRule<TRequest, Response<TodoItemEntity>>> rules)
        {
            await _rulesEngine.ProcessAsync(request, response, rules);
            var rulesFailed = rules.Any(rule => rule.Failed);
            if (rulesFailed && response.IsStatusNotSet()) response.SetStatusToBadRequest();
            return rulesFailed;
        }
    }
}
