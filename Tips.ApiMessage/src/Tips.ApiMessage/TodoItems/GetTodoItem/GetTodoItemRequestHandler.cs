using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.GetTodoItem
{
    public class GetTodoItemRequestHandler : IRequestHandler<GetTodoItemRequest, GetTodoItemResponse>
    {
        private readonly TodoContext _context;

        public GetTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<GetTodoItemResponse> Handle(GetTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(request.Id);

            return todoItem != null ? Found(todoItem) : NotFound();
        }

        private static GetTodoItemResponse Found(TodoItemEntity todoItem) =>
            new GetTodoItemResponse
            {
                ApiMessage = new ApiMessage.Models.ApiMessage
                {
                    Status = (int) HttpStatusCode.OK
                },
                TodoItem = ItemToResponse(todoItem)
            };

        private static GetTodoItemResponse NotFound() =>
            new GetTodoItemResponse
            {
                ApiMessage = new ApiMessage.Models.ApiMessage
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
