using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Context;
using Tips.ApiMessage.Models;

namespace Tips.ApiMessage.Handlers
{
    public class CreateTodoItemRequestHandler : IRequestHandler<CreateTodoItemCommand, CreateTodoItemResponse>
    {
        private readonly TodoContext _context;

        public CreateTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<CreateTodoItemResponse> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
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
                ApiMessage = new Messages.ApiMessage
                {
                    Status = (int) HttpStatusCode.Created
                },
                Id = todoItemEntity.Id,
            };
        }
    }
}
