using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;

namespace Tips.TodoItems.Rules.UpdateRules
{
    public class TodoItemNotFoundRule : BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>
    {
        private readonly TodoContext _context;

        public TodoItemNotFoundRule(TodoContext context) => _context = context;

        protected override async Task ProcessRuleAsync(UpdateTodoItemRequest request, Response<TodoItemEntity> response)
        {
            if (!await TodoItemExists(request.Id))
            {
                response.Add(NotFoundNotification(request.Id));
                response.SetStatusToNotFound();
                ContinueProcessing = false;
                RuleFailed();
                return;
            }

            RulePassed();
        }

        private async Task<bool> TodoItemExists(long id) => await _context.TodoItems.AnyAsync(e => e.Id == id);

        internal const string NotFoundNotificationId = "9E1A675F-3073-4D78-9A22-317ECB1D88DC";
        private static Notification NotFoundNotification(long id) =>
            Notification.CreateError(NotFoundNotificationId, $"TodoItem {id} was not found.");
    }
}
