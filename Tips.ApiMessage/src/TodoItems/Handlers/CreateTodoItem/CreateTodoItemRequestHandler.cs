using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tips.Pipeline;
using Tips.Pipeline.Extensions;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Mappers;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.CreateTodoItem
{
    internal class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly IRulesEngine _rulesEngine;
        private readonly IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> _saveRulesFactory;
        private readonly ICreateTodoItemRepository _createTodoItemRepository;
        private readonly ILogger<CreateTodoItemRequestHandler> _logger;

        public CreateTodoItemRequestHandler(IRulesEngine rulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> saveRulesFactory,
            ICreateTodoItemRepository createTodoItemRepository,
            ILogger<CreateTodoItemRequestHandler> logger)
        {
            _rulesEngine = rulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _createTodoItemRepository = createTodoItemRepository;
            _logger = logger;
        }

        public async Task<Response<TodoItem>> HandleAsync(CreateTodoItemRequest request)
        {
            var todoItemEntityResponse = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            await _rulesEngine.ProcessAsync(request, todoItemEntityResponse, _saveRulesFactory.Create().ToList());
            if (todoItemEntityResponse.HasErrors()) return new Response<TodoItem>(todoItemEntityResponse.Notifications);

            // Command.  Save the data.
            await _createTodoItemRepository.SaveAsync(todoItemEntityResponse);

            LogTodoItemEntityResponse(todoItemEntityResponse);

            return new Response<TodoItem>(todoItemEntityResponse.Notifications, TodoItemMapper.MapToTodoItem(todoItemEntityResponse.Item));
        }

        private void LogTodoItemEntityResponse(Response<TodoItemEntity> todoItemEntityResponse) =>
            _logger.LogAction("Created TodoItemEntity", () =>
                _logger.LogInformation("{TodoItemEntityResponse}", JsonSerializer.Serialize(todoItemEntityResponse)));
    }
}
