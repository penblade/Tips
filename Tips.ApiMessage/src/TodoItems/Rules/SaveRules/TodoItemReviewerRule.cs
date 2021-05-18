using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Rules.SaveRules
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
                response.Notifications.Add(TodoItemReviewerIsNullNotification(request.Item.Priority));
                Fail();
                return Task.CompletedTask;
            }

            Pass();
            return Task.CompletedTask;
        }

        internal const string TodoItemReviewerIsNullNotificationId = "1FC6B1C0-B72A-4F2F-ADBC-C059382D4363";
        private static Notification TodoItemReviewerIsNullNotification(int priority)
        {
            var notification = Notification.CreateError(TodoItemReviewerIsNullNotificationId,
                "TodoItem Reviewer could not be determined.");

            notification.Notifications.Add(TodoItemReviewerIsNullReason1Notification());
            notification.Notifications.Add(TodoItemReviewerIsNullReason2Notification(priority));
            notification.Notifications.Add(TodoItemReviewerIsNullReason3Notification());
            notification.Notifications.Add(TodoItemReviewerIsNullReason4Notification());

            return notification;
        }

        internal const string TodoItemReviewerIsNullReason1NotificationId = "4A371CEB-5353-41A0-8712-7C433E7019BB";

        private static Notification TodoItemReviewerIsNullReason1Notification() => Notification.CreateInfo(TodoItemReviewerIsNullReason1NotificationId, "Reviewer is based on priority.");

        internal const string TodoItemReviewerIsNullReason2NotificationId = "567320AA-6CA3-4139-A43F-29A1F27B3A2E";

        private static Notification TodoItemReviewerIsNullReason2Notification(int priority) => Notification.CreateInfo(TodoItemReviewerIsNullReason2NotificationId, $"An administrator needs to assign a reviewer to priority {priority}.");

        internal const string TodoItemReviewerIsNullReason3NotificationId = "ED912757-078C-4F5E-AFB6-96C612497DD8";

        private static Notification TodoItemReviewerIsNullReason3Notification() => Notification.CreateInfo(TodoItemReviewerIsNullReason3NotificationId, "Please contact your administrator with this error.");

        internal const string TodoItemReviewerIsNullReason4NotificationId = "DB5BE8D0-8BD8-4713-B59C-49A2D3EE73A4";

        private static Notification TodoItemReviewerIsNullReason4Notification() => Notification.CreateInfo(TodoItemReviewerIsNullReason4NotificationId, "This is an example of nested notifications.");
    }
}
