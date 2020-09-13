using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemDescriptionRule : BaseRule
    {
        protected override void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            if (string.IsNullOrEmpty(request.TodoItem.Description))
            {
                response.Add(TodoItemDescriptionWasNotProvidedNotification());
                return;
            }

            response.Result.Description = request.TodoItem.Description;
        }

        internal const string TodoItemDescriptionWasNotProvidedNotificationId = "54BD317D-60CD-4BDE-B52D-CF7D0A1D9D38";
        private static Notification TodoItemDescriptionWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemDescriptionWasNotProvidedNotificationId, "TodoItem Description was not provided.");
    }
}
