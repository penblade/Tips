using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.TodoItems.Context;

namespace Tips.TodoItems.Handlers.DeleteTodoItem
{
    internal class DeleteTodoItemRequestHandler : IRequestHandler<DeleteTodoItemRequest, Response>
    {
        private readonly TodoContext _context;

        public DeleteTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response> HandleAsync(DeleteTodoItemRequest request)
        {
            var todoItemEntity = await _context.TodoItems.FindAsync(request.Id);

            if (todoItemEntity == null) return new Response(TodoItemNotFoundNotification(request.Id));

            _context.TodoItems.Remove(todoItemEntity);
            await _context.SaveChangesAsync();

            return new Response();
        }

        internal const string TodoItemNotFoundNotificationId = "44799F8F-C7CE-4392-8709-2824899E486C";
        private static Notification TodoItemNotFoundNotification(long id) =>
            NotFoundNotification.Create(TodoItemNotFoundNotificationId, $"TodoItem {id} was not found.");
    }
}
