using System.Threading;
using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.TodoItems.Context;
using Tips.TodoItems.Mappers;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.GetTodoItem
{
    internal class GetTodoItemRequestHandler : IRequestHandler<GetTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;

        public GetTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response<TodoItem>> HandleAsync(GetTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemEntity = await _context.TodoItems.FindAsync(request.Id);

            return todoItemEntity == null
                ? new Response<TodoItem>(TodoItemNotFoundNotification(request.Id))
                : new Response<TodoItem>(TodoItemMapper.MapToTodoItem(todoItemEntity));
        }

        internal const string TodoItemNotFoundNotificationId = "D2B34535-0896-491B-9E6A-6D9F6575DD9E";
        private static Notification TodoItemNotFoundNotification(long id) =>
            NotFoundNotification.Create(TodoItemNotFoundNotificationId, $"TodoItem {id} was not found.");
    }
}
