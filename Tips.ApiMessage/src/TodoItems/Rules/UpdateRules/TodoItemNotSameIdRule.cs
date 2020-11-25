using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;

namespace Tips.TodoItems.Rules.UpdateRules
{
    internal class TodoItemNotSameIdRule : BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>
    {
        protected override Task ProcessRuleAsync(UpdateTodoItemRequest request, Response<TodoItemEntity> response)
        {
            if (request.Id != request.Item.Id)
            {
                response.Notifications.Add(NotSameIdNotification(request.Id, request.Item.Id));
                ContinueProcessing = false;
                RuleFailed();
                return Task.CompletedTask;
            }
            RulePassed();
            return Task.CompletedTask;
        }

        internal const string NotSameIdNotificationId = "38EFC3AD-7A84-4D49-85F5-E325125A6EE1";
        private static Notification NotSameIdNotification(long requestId, long todoItemId) =>
            Notification.CreateError(NotSameIdNotificationId, $"TodoItem {requestId} does not match {todoItemId}.");
    }
}
