using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Context;
using Tips.ApiMessage.Models;

namespace Tips.ApiMessage.Handlers
{
    public class GetTodoItemRequestHandler : IRequestHandler<TodoItemQuery, TodoItemResponse>
    {
        private readonly TodoContext _context;

        public GetTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<TodoItemResponse> Handle(TodoItemQuery request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(request.Id);

            return todoItem != null ? Found(todoItem) : NotFound();
        }

        private static TodoItemResponse Found(TodoItemEntity todoItem) =>
            new TodoItemResponse
            {
                ApiMessage = new Messages.ApiMessage
                {
                    Status = (int) HttpStatusCode.OK
                },
                TodoItem = ItemToResponse(todoItem)
            };

        private static TodoItemResponse NotFound() =>
            new TodoItemResponse
            {
                ApiMessage = new Messages.ApiMessage
                {
                    Status = (int) HttpStatusCode.NotFound
                },
                TodoItem = null
            };

        private static TodoItem ItemToResponse(TodoItemEntity todoItem) =>
            new TodoItem
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
