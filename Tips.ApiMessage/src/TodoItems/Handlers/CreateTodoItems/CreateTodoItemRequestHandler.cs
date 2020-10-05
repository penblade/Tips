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

        public async Task<Response<TodoItem>> HandleAsync(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemEntityResponse = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            await _todoItemRulesEngine.ProcessAsync(request, todoItemEntityResponse, _saveRulesFactory.Create().ToList());
            if (todoItemEntityResponse.HasErrors()) return new Response<TodoItem>(todoItemEntityResponse.Notifications);

            // Command.  Save the data.
            await _createTodoItemRepository.SaveAsync(todoItemEntityResponse, cancellationToken);

            return new Response<TodoItem>(todoItemEntityResponse.Notifications, TodoItemMapper.MapToTodoItem(todoItemEntityResponse.Item));
        }
    }
}
