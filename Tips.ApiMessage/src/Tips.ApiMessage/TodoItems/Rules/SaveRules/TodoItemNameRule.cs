using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemNameRule : BaseRule<Request<TodoItem>, Response<TodoItem>>
    {
        public TodoItemNameRule() => RequiredRules.AddRange(new []{typeof(RequestRule), typeof(ResponseRule)});

        protected override void ProcessRule(Request<TodoItem> request, Response<TodoItem> response)
        {
            if (string.IsNullOrEmpty(request.Item.Name))
            {
                response.Add(TodoItemNameWasNotProvidedNotification());
                RuleFailed();
                return;
            }

            response.Result.Name = request.Item.Name;
            RulePassed();
        }

        internal const string TodoItemNameWasNotProvidedNotificationId = "148877DF-F147-413F-97AA-F306A36BCBE1";
        private static Notification TodoItemNameWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemNameWasNotProvidedNotificationId, "TodoItem Name was not provided.");
    }
}
