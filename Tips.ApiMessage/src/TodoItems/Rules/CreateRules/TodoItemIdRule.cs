using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.CreateTodoItem;

namespace Tips.TodoItems.Rules.CreateRules
{
    internal class TodoItemIdRule : BaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>
    {
        protected override Task ProcessRuleAsync(CreateTodoItemRequest request, Response<TodoItemEntity> response)
        {
            if (request.Item.Id != 0)
            {
                response.Notifications.Add(TodoItemIdIsNotEmptyRuleNotification());
                Fail();
                ContinueProcessing = false;
                return Task.CompletedTask;
            }

            Pass();
            return Task.CompletedTask;
        }

        internal const string TodoItemIdIsNotEmptyRuleNotificationId = "8E74A5CE-4DFD-42A8-9CA8-39BF220DD6AB";

        private static Notification TodoItemIdIsNotEmptyRuleNotification() =>
            Notification.CreateError(TodoItemIdIsNotEmptyRuleNotificationId, "Do not send the TodoItem.ID when creating a new TodoItem.");
    }
}
