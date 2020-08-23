using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    public class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;

        public CreateTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response<TodoItem>> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemEntity = new TodoItemEntity
            {
                IsComplete = request.TodoItem.IsComplete,
                Name = request.TodoItem.Name
            };

            await _context.TodoItems.AddAsync(todoItemEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Created(todoItemEntity);
        }

        private static Response<TodoItem> Created(TodoItemEntity todoItemEntity) =>
            new Response<TodoItem>
            {
                Notifications = null,
                Status = (int) HttpStatusCode.Created,
                // TraceId = TraceId,
                Result = TodoItemMapper.ItemToResponse(todoItemEntity)
            };
    }
}
