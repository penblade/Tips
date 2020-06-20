using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;

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

            return new CreateTodoItemResponse
            {
                ApiMessage = new ApiMessage.Models.ApiMessage
                {
                    Status = (int) HttpStatusCode.Created
                },
                Id = todoItemEntity.Id,
            };
        }
    }
}
