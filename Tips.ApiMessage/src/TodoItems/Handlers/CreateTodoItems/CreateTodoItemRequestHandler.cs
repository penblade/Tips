using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Mappers;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.CreateTodoItems
{
    public class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
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

        public async Task<Response<TodoItem>> HandleAsync(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            if (await ProcessRulesAsync(request, response, _saveRulesFactory.Create().ToList())) return null;

            // Command.  Save the data.
            await _createTodoItemRepository.SaveAsync(response, cancellationToken);

            return MapToResponseTodoItem(response);
        }

        private static Response<TodoItem> MapToResponseTodoItem(Response<TodoItemEntity> response) =>
            new Response<TodoItem>
            {
                Item = TodoItemMapper.MapToTodoItem(response.Item),
                Notifications = response.Notifications,
                Status = response.Status
            };

        private async Task<bool> ProcessRulesAsync(Request<TodoItem> request, Response<TodoItemEntity> response, IReadOnlyCollection<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>> rules)
        {
            await _todoItemRulesEngine.ProcessAsync(request, response, rules);
            var rulesFailed = rules.Any(rule => rule.Failed);
            if (rulesFailed && response.IsStatusNotSet()) response.SetStatusToBadRequest();
            return rulesFailed;
        }
    }
}
