using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Rules.SaveRules
{
    public class RequestRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        protected override Task ProcessRuleAsync(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            if (request.Item == null)
            {
                response.Add(TodoItemWasNotProvidedNotification());
                RuleFailed();
                ContinueProcessing = false;
                return Task.CompletedTask;
            }

            RulePassed();
            return Task.CompletedTask;
        }

        internal const string TodoItemWasNotProvidedNotificationId = "DC02BFB8-F28D-4CA7-8EFB-74A4E89C1558";
        private static Notification TodoItemWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemWasNotProvidedNotificationId, "TodoItem was not provided.");
    }
}
