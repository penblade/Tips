using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    public class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse>
    {
        private readonly TodoContext _context;

        public CreateTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<CreateTodoItemResponse> Handle(CreateTodoItemRequest request, CancellationToken cancellationToken)
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

        private static CreateTodoItemResponse Created(TodoItemEntity todoItemEntity) =>
            new CreateTodoItemResponse
            {
                Notifications = null,
                Status = (int) HttpStatusCode.Created,
                // TraceId = TraceId,
                TodoItem = TodoItemMapper.ItemToResponse(todoItemEntity)
            };
    }
}
