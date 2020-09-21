using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Mappers;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    internal class UpdateTodoItemRepository : IUpdateTodoItemRepository
    {
        private readonly TodoContext _context;

        public UpdateTodoItemRepository(TodoContext context) => _context = context;

        public async Task Save(Response<TodoItemEntity> response, CancellationToken cancellationToken)
        {
            TodoItemEntity todoItemEntity;
            try
            {
                todoItemEntity = await _context.TodoItems.FindAsync(response.Item.Id);
                TodoItemMapper.MapToTodoItemEntity(response.Item, todoItemEntity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(response.Item.Id))
            {
                response.Add(NotFoundWhenSavingNotification(response.Item.Id));
                response.SetStatusToNotFound();
                return;
            }

            response.Item = todoItemEntity;
            response.SetStatusToNoContent();
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        internal const string NotFoundWhenSavingNotificationId = "8FD46D5D-1CB3-4ECB-B27B-724813A0406C";

        private static Notification NotFoundWhenSavingNotification(long id) =>
            Notification.CreateError(NotFoundWhenSavingNotificationId, $"TodoItem {id} was not found when saving.");
    }
}
