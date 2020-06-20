using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;

namespace Tips.ApiMessage.TodoItems.DeleteTodoItems
{
    public class DeleteTodoItemRequestHandler : IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse>
    {
        private readonly TodoContext _context;

        public DeleteTodoItemRequestHandler(TodoContext context) => _context = context;

        public async Task<DeleteTodoItemResponse> Handle(DeleteTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItem = await _context.TodoItems.FindAsync(request.Id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private static DeleteTodoItemResponse NoContent() =>
            new DeleteTodoItemResponse
            {
                ApiMessage = new Contracts.ApiMessage
                {
                    Status = (int) HttpStatusCode.NoContent
                }
            };

        private static DeleteTodoItemResponse NotFound() =>
            new DeleteTodoItemResponse
            {
                ApiMessage = new Contracts.ApiMessage
                {
                    Status = (int) HttpStatusCode.NotFound
                }
            };
    }
}
