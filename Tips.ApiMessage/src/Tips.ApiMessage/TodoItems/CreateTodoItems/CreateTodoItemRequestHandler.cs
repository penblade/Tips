using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    internal class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;
        private readonly ITodoItemRulesEngine _todoItemRulesEngine;
        private readonly IRulesFactory _rulesFactory;

        public CreateTodoItemRequestHandler(TodoContext context, ITodoItemRulesEngine todoItemRulesEngine, IRulesFactory rulesFactory)
        {
            _context = context;
            _todoItemRulesEngine = todoItemRulesEngine;
            _rulesFactory = rulesFactory;
        }

        public async Task<Response<TodoItem>> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            // Query. Apply all validation and modification rules.  These rules can only query the database.
            var response = new Response<TodoItem>();
            _todoItemRulesEngine.ProcessRules(request, response, _rulesFactory.Create());

            if (response.HasErrors())
            {
                response.SetStatusToBadRequest();
                return response;
            }

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
