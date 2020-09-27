using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemReviewerRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        public TodoItemReviewerRule() => RequiredRules.AddRange(new[] { typeof(RequestRule), typeof(ResponseRule), typeof(TodoItemPriorityRule) });

        protected override Task ProcessRuleAsync(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            // This would be a database lookup, but simplified for this example.
            response.Item.Reviewer = request.Item.Priority switch
            {
                1 => "Peter",
                2 => "Lois",
                3 => "Brian",
                _ => null
            };

            if (string.IsNullOrEmpty(response.Item.Reviewer))
            {
                response.Add(TodoItemReviewerIsNullNotification());
                response.SetStatusToBadRequest();
                RuleFailed();
                return Task.CompletedTask;
            }

            RulePassed();
            return Task.CompletedTask;
        }

        internal const string TodoItemReviewerIsNullNotificationId = "1FC6B1C0-B72A-4F2F-ADBC-C059382D4363";
        private static Notification TodoItemReviewerIsNullNotification() =>
            Notification.CreateError(TodoItemReviewerIsNullNotificationId, "TodoItem Reviewer could not be determined.");
    }
}
