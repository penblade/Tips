using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;

namespace Tips.ApiMessage.TodoItems.DeleteTodoItems
{
    public class DeleteTodoItemRequestHandler : IRequestHandler<DeleteTodoItemRequest, Response>
    {
        private readonly TodoContext _context;

        public DeleteTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response> Handle(DeleteTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(request.Id);
            if (todoItem == null) return NotFound();

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private static Response NoContent() =>
            new Response
            {
                Notifications = null,
                Status = (int) HttpStatusCode.NoContent,
            };

        private static Response NotFound() =>
            new Response
            {
                Notifications = null,
                Status = (int) HttpStatusCode.NotFound,
            };
    }
}
