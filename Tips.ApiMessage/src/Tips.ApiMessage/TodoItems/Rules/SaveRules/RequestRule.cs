using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class RequestRule : BaseRule<SaveTodoItemRequest, Response<TodoItem>>
    {
        protected override void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            if (request.TodoItem == null)
            {
                response.Add(TodoItemWasNotProvidedNotification());
                ContinueProcessing = false;
            }
        }

        internal const string TodoItemWasNotProvidedNotificationId = "DC02BFB8-F28D-4CA7-8EFB-74A4E89C1558";
        private static Notification TodoItemWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemWasNotProvidedNotificationId, "TodoItem was not provided.");
    }
}
