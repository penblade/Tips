using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Context;
using Tips.ApiMessage.Models;

namespace Tips.ApiMessage.Handlers
{
    public class GetTodoItemRequestHandler<TRequest, TResponse> : IRequestHandler<TodoItemQuery, Response>
    {
        private readonly TodoContext _context;

        public GetTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response> Handle(TodoItemQuery request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(request.Id);

            //if (todoItem == null) return NotFound();

            return new TodoItemResponse
            {
                ApiMessage = new Messages.ApiMessage
                {
                    Status = (int) HttpStatusCode.OK
                },
                TodoItem = ItemToResponse(todoItem)
            };
        }

        private static TodoItem ItemToResponse(TodoItemEntity todoItem) =>
            new TodoItem
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
