using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.GuardClauses;
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
            Guard.AgainstNull(response, nameof(response));
            Guard.AgainstNull(response.Item, nameof(response.Item));

            try
            {
                var todoItemEntity = await _context.TodoItems.FindAsync(response.Item.Id);
                if (todoItemEntity == null)
                {
                    response.Notifications.Add(TodoItemNotFoundNotification(response.Item.Id));
                    return;
                }
                TodoItemMapper.MapToTodoItemEntity(response.Item, todoItemEntity);
                await _context.SaveChangesAsync();
                response.Item = todoItemEntity;
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(response.Item.Id))
            {
                response.Notifications.Add(TodoItemNotFoundWhenSavingNotification(response.Item.Id));
            }
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        internal const string TodoItemNotFoundNotificationId = "E74FCBD2-B539-4582-9217-0FDC4E1BB27C";

        private static Notification TodoItemNotFoundNotification(long id) =>
            NotFoundNotification.Create(TodoItemNotFoundNotificationId, $"TodoItem {id} was not found.");

        internal const string TodoItemNotFoundWhenSavingNotificationId = "8FD46D5D-1CB3-4ECB-B27B-724813A0406C";

        private static Notification TodoItemNotFoundWhenSavingNotification(long id) =>
            NotFoundNotification.Create(TodoItemNotFoundWhenSavingNotificationId, $"TodoItem {id} was not found when saving.");
    }
}
