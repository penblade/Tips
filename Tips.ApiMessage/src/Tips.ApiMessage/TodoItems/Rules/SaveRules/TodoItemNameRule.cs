using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemNameRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        public TodoItemNameRule() => RequiredRules.AddRange(new []{typeof(RequestRule), typeof(ResponseRule)});

        protected override Task ProcessRuleAsync(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            if (string.IsNullOrEmpty(request.Item.Name))
            {
                response.Add(TodoItemNameWasNotProvidedNotification());
                RuleFailed();
                return Task.CompletedTask;
            }

            response.Item.Name = request.Item.Name;
            RulePassed();
            return Task.CompletedTask;
        }

        internal const string TodoItemNameWasNotProvidedNotificationId = "148877DF-F147-413F-97AA-F306A36BCBE1";
        private static Notification TodoItemNameWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemNameWasNotProvidedNotificationId, "TodoItem Name was not provided.");
    }
}
