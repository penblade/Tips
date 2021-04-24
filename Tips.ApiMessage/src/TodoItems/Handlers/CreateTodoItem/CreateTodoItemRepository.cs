using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;

namespace Tips.TodoItems.Handlers.CreateTodoItem
{
    internal class CreateTodoItemRepository : ICreateTodoItemRepository
    {
        private readonly TodoContext _context;

        public CreateTodoItemRepository(TodoContext context) => _context = context;

        public async Task SaveAsync(Response<TodoItemEntity> response)
        {
            await _context.TodoItems.AddAsync(response.Item);
            await _context.SaveChangesAsync();
        }
    }
}
