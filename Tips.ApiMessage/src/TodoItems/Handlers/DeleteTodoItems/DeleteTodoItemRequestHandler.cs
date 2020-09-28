using System.Threading;
using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.TodoItems.Context;

namespace Tips.TodoItems.Handlers.DeleteTodoItems
{
    public class DeleteTodoItemRequestHandler : IRequestHandler<DeleteTodoItemRequest, Response>
    {
        private readonly TodoContext _context;

        public DeleteTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<Response> HandleAsync(DeleteTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response();
            var todoItem = await _context.TodoItems.FindAsync(request.Id);
            if (todoItem == null)
            {
                response.SetStatusToNotFound();
                return response;
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync(cancellationToken);

            response.SetStatusToNoContent();
            return response;
        }
    }
}
