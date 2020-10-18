using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Mappers;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.CreateTodoItems
{
    internal class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly IRulesEngine _todoItemRulesEngine;
        private readonly IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> _saveRulesFactory;
        private readonly ICreateTodoItemRepository _createTodoItemRepository;
        private readonly ILogger<CreateTodoItemRequestHandler> _logger;

        public CreateTodoItemRequestHandler(IRulesEngine todoItemRulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> saveRulesFactory,
            ICreateTodoItemRepository createTodoItemRepository,
            ILogger<CreateTodoItemRequestHandler> logger)
        {
            _todoItemRulesEngine = todoItemRulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _createTodoItemRepository = createTodoItemRepository;
            _logger = logger;
        }

        public async Task<Response<TodoItem>> HandleAsync(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemEntityResponse = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            await _todoItemRulesEngine.ProcessAsync(request, todoItemEntityResponse, _saveRulesFactory.Create().ToList());
            if (todoItemEntityResponse.HasErrors()) return new Response<TodoItem>(todoItemEntityResponse.Notifications);

            // Command.  Save the data.
            _logger.LogInformation(CreateLogMessage(JsonSerializer.Serialize(todoItemEntityResponse)));
            await _createTodoItemRepository.SaveAsync(todoItemEntityResponse, cancellationToken);

            return new Response<TodoItem>(todoItemEntityResponse.Notifications, TodoItemMapper.MapToTodoItem(todoItemEntityResponse.Item));
        }

        private static string CreateLogMessage(string response) => @$"TraceId: {Tracking.TraceId} | Created: {LogFormatter.FormatForLogging(response)}";
    }
}
