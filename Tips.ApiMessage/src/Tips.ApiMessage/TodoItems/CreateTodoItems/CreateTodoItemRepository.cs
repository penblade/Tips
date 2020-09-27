using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Context.Models;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    internal class CreateTodoItemRepository : ICreateTodoItemRepository
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
