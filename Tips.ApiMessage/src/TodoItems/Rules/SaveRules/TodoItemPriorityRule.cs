using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Rules.SaveRules
{
    public class TodoItemPriorityRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        public TodoItemPriorityRule() => RequiredRules.AddRange(new[] { typeof(RequestRule), typeof(ResponseRule) });

        protected override Task ProcessRuleAsync(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            if (request.Item.Priority < 1 || request.Item.Priority > 3)
            {
                response.Add(TodoItemPriorityIsNotInRangeNotification());
                RuleFailed();
                return Task.CompletedTask;
            }

            response.Item.Priority = request.Item.Priority;
            RulePassed();
            return Task.CompletedTask;
        }

        internal const string TodoItemPriorityIsNotInRangeNotificationId = "C5E1E6F4-D241-4D82-A4C5-832E3C6C1816";
        private static Notification TodoItemPriorityIsNotInRangeNotification() =>
            Notification.CreateError(TodoItemPriorityIsNotInRangeNotificationId, "TodoItem Priority must be between 1 - 3.");
    }
}
