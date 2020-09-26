using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemDescriptionRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        public TodoItemDescriptionRule() => RequiredRules.AddRange(new[] { typeof(RequestRule), typeof(ResponseRule) });

        protected override Task ProcessRule(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            if (string.IsNullOrEmpty(request.Item.Description))
            {
                response.Add(TodoItemDescriptionWasNotProvidedNotification());
                RuleFailed();
                return Task.CompletedTask;
            }

            response.Item.Description = request.Item.Description;
            RulePassed();
            return Task.CompletedTask;
        }

        internal const string TodoItemDescriptionWasNotProvidedNotificationId = "54BD317D-60CD-4BDE-B52D-CF7D0A1D9D38";
        private static Notification TodoItemDescriptionWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemDescriptionWasNotProvidedNotificationId, "TodoItem Description was not provided.");
    }
}
