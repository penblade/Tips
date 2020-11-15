using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.Pipeline;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Mappers;

namespace Tips.TodoItems.Handlers.UpdateTodoItem
{
    internal class UpdateTodoItemRepository : IUpdateTodoItemRepository
    {
        private readonly TodoContext _context;

        public UpdateTodoItemRepository(TodoContext context) => _context = context;

        public async Task SaveAsync(Response<TodoItemEntity> response)
        {
            TodoItemEntity todoItemEntity;
            try
            {
                todoItemEntity = await _context.TodoItems.FindAsync(response.Item.Id);
                TodoItemMapper.MapToTodoItemEntity(response.Item, todoItemEntity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(response.Item.Id))
            {
                response.Add(TodoItemNotFoundWhenSavingNotification(response.Item.Id));
                return;
            }

            response.Item = todoItemEntity;
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        internal const string TodoItemNotFoundWhenSavingNotificationId = "8FD46D5D-1CB3-4ECB-B27B-724813A0406C";

        private static Notification TodoItemNotFoundWhenSavingNotification(long id) =>
            NotFoundNotification.Create(TodoItemNotFoundWhenSavingNotificationId, $"TodoItem {id} was not found when saving.");
    }
}
