using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;

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

        private static GetTodoItemResponse Found(TodoItemEntity todoItemEntity) =>
            new GetTodoItemResponse
            {
                Notifications = null,
                Status = (int) HttpStatusCode.OK,
                // TraceId = TraceId,
                TodoItem = TodoItemMapper.ItemToResponse(todoItemEntity)
            };

        private static GetTodoItemResponse NotFound() =>
            new GetTodoItemResponse
            {
                Notifications = null,
                Status = (int) HttpStatusCode.NotFound,
                // TraceId = TraceId,
                TodoItem = null
            };
    }
}
