using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    internal class CreateTodoItemRepository : ICreateTodoItemRepository
    {
        private readonly TodoContext _context;

        public CreateTodoItemRepository(TodoContext context) => _context = context;

        public async Task<Response<TodoItem>> Save(Response<TodoItem> response, CancellationToken cancellationToken)
        {
            var todoItemEntity = TodoItemMapper.MapToTodoItemEntity(response.Result);

            await _context.TodoItems.AddAsync(todoItemEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            response.Result = TodoItemMapper.MapToTodoItem(todoItemEntity);
            response.SetStatusToCreated();
            return response;
        }
    }
}
