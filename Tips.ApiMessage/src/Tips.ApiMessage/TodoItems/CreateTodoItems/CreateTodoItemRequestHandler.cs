using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    internal class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly IRulesEngine _todoItemRulesEngine;
        private readonly IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> _saveRulesFactory;
        private readonly ICreateTodoItemRepository _createTodoItemRepository;

        public CreateTodoItemRequestHandler(IRulesEngine todoItemRulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> saveRulesFactory,
            ICreateTodoItemRepository createTodoItemRepository)
        {
            _todoItemRulesEngine = todoItemRulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _createTodoItemRepository = createTodoItemRepository;
        }

        public async Task<Response<TodoItem>> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            if (await ProcessRules(request, response, _saveRulesFactory.Create().ToList())) return null;

            // Command.  Save the data.
            await _createTodoItemRepository.Save(response, cancellationToken);

            return MapToResponseTodoItem(response);
        }

        private static Response<TodoItem> MapToResponseTodoItem(Response<TodoItemEntity> response) =>
            new Response<TodoItem>
            {
                Item = TodoItemMapper.MapToTodoItem(response.Item),
                Notifications = response.Notifications,
                Status = response.Status
            };

        private async Task<bool> ProcessRules(Request<TodoItem> request, Response<TodoItemEntity> response, IReadOnlyCollection<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>> rules)
        {
            await _todoItemRulesEngine.Process(request, response, rules);
            var rulesFailed = rules.Any(rule => rule.Failed);
            if (rulesFailed && response.IsStatusNotSet()) response.SetStatusToBadRequest();
            return rulesFailed;
        }
    }
}
