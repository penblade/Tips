using System.Threading;
using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;

namespace Tips.TodoItems.Handlers.CreateTodoItems
{
    public class CreateTodoItemRepository : ICreateTodoItemRepository
    {
        private readonly TodoContext _context;

        public CreateTodoItemRepository(TodoContext context) => _context = context;

        public async Task SaveAsync(Response<TodoItemEntity> response, CancellationToken cancellationToken)
        {
            await _context.TodoItems.AddAsync(response.Item, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.SetStatusToCreated();
        }
    }
}
