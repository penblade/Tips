using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemPriorityRule : BaseRule<SaveTodoItemRequest, Response<TodoItem>>
    {
        public TodoItemPriorityRule() => RequiredRules.AddRange(new[] { typeof(RequestRule), typeof(ResponseRule) });

        protected override void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            if (request.TodoItem.Priority < 1 || request.TodoItem.Priority > 3)
            {
                response.Add(TodoItemPriorityIsNotInRangeNotification());
                RuleFailed();
                return;
            }

            response.Result.Priority = request.TodoItem.Priority;
            RulePassed();
        }

        internal const string TodoItemPriorityIsNotInRangeNotificationId = "C5E1E6F4-D241-4D82-A4C5-832E3C6C1816";
        private static Notification TodoItemPriorityIsNotInRangeNotification() =>
            Notification.CreateError(TodoItemPriorityIsNotInRangeNotificationId, "TodoItem Priority must be between 1 - 3.");
    }
}
