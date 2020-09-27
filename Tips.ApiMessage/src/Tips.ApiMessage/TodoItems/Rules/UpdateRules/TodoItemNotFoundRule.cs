using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Rules.UpdateRules
{
    internal class TodoItemNotFoundRule : BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>
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
