using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    internal class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly IRulesEngine _todoItemRulesEngine;
        private readonly IRulesFactory<Request<TodoItem>, Response<TodoItem>> _saveRulesFactory;
        private readonly ICreateTodoItemRepository _createTodoItemRepository;

        public CreateTodoItemRequestHandler(IRulesEngine todoItemRulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItem>> saveRulesFactory,
            ICreateTodoItemRepository createTodoItemRepository)
        {
            _todoItemRulesEngine = todoItemRulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _createTodoItemRepository = createTodoItemRepository;
        }

        public async Task<Response<TodoItem>> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItem>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            if (ProcessRules(request, response, _saveRulesFactory.Create().ToList())) return null;

            // Command.  Save the data.
            return await _createTodoItemRepository.Save(response, cancellationToken);
        }

        private bool ProcessRules(Request<TodoItem> request, Response<TodoItem> response, IReadOnlyCollection<BaseRule<Request<TodoItem>, Response<TodoItem>>> rules)
        {
            _todoItemRulesEngine.Process(request, response, rules);
            var rulesFailed = rules.Any(rule => rule.Failed);
            if (rulesFailed && response.IsStatusNotSet()) response.SetStatusToBadRequest();
            return rulesFailed;
        }
    }
}
