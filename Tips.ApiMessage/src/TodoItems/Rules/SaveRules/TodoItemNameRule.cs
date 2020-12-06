using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Rules.SaveRules
{
    internal class TodoItemNameRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        public TodoItemNameRule() => RequiredRules.AddRange(new []{typeof(RequestRule), typeof(ResponseRule)});

        protected override Task ProcessRuleAsync(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            if (string.IsNullOrEmpty(request.Item.Name))
            {
                response.Notifications.Add(TodoItemNameWasNotProvidedNotification());
                Fail();
                return Task.CompletedTask;
            }

            response.Item.Name = request.Item.Name;
            Pass();
            return Task.CompletedTask;
        }

        internal const string TodoItemNameWasNotProvidedNotificationId = "148877DF-F147-413F-97AA-F306A36BCBE1";
        private static Notification TodoItemNameWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemNameWasNotProvidedNotificationId, "TodoItem Name was not provided.");
    }
}
