using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    internal class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;
        private readonly IRulesEngine _todoItemRulesEngine;
        private readonly IRulesFactory<SaveTodoItemRequest, Response<TodoItem>> _saveRulesFactory;

        public CreateTodoItemRequestHandler(TodoContext context, IRulesEngine todoItemRulesEngine,
            IRulesFactory<SaveTodoItemRequest, Response<TodoItem>> saveRulesFactory)
        {
            _context = context;
            _todoItemRulesEngine = todoItemRulesEngine;
            _saveRulesFactory = saveRulesFactory;
        }

        public async Task<Response<TodoItem>> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            // Query. Apply all validation and modification rules.  These rules can only query the database.
            var response = new Response<TodoItem>();
            _todoItemRulesEngine.Process(request, response, _saveRulesFactory.Create());

            if (response.HasErrors()) response.SetStatusToBadRequest();
            if (response.HasErrors()) return response;

            // Command.  Save the data.
            var todoItem = await Save(request, cancellationToken);

            response.SetStatusToCreated();
            response.Result = todoItem;
            return response;
        }

        private async Task<TodoItem> Save(SaveTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemEntity = TodoItemMapper.MapToTodoItemEntity(request.TodoItem);

            await _context.TodoItems.AddAsync(todoItemEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return TodoItemMapper.MapToTodoItem(todoItemEntity);
        }
    }
}
