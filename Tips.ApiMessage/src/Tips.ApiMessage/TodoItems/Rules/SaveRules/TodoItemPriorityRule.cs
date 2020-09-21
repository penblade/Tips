using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemPriorityRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        public TodoItemPriorityRule() => RequiredRules.AddRange(new[] { typeof(RequestRule), typeof(ResponseRule) });

        protected override void ProcessRule(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            if (request.Item.Priority < 1 || request.Item.Priority > 3)
            {
                response.Add(TodoItemPriorityIsNotInRangeNotification());
                RuleFailed();
                return;
            }

            response.Item.Priority = request.Item.Priority;
            RulePassed();
        }

        internal const string TodoItemPriorityIsNotInRangeNotificationId = "C5E1E6F4-D241-4D82-A4C5-832E3C6C1816";
        private static Notification TodoItemPriorityIsNotInRangeNotification() =>
            Notification.CreateError(TodoItemPriorityIsNotInRangeNotificationId, "TodoItem Priority must be between 1 - 3.");
    }
}
