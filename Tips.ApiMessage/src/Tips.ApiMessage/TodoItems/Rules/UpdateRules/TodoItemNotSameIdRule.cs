using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Rules.UpdateRules
{
    internal class TodoItemNotSameIdRule : BaseRule<UpdateTodoItemRequest, Response<TodoItem>>
    {
        protected override void ProcessRule(UpdateTodoItemRequest request, Response<TodoItem> response)
        {
            if (request.Id != request.Item.Id)
            {
                response.Add(NotSameIdNotification(request.Id, request.Item.Id));
                ContinueProcessing = false;
                RuleFailed();
                return;
            }
            RulePassed();
        }

        internal const string NotSameIdNotificationId = "38EFC3AD-7A84-4D49-85F5-E325125A6EE1";
        private static Notification NotSameIdNotification(long requestId, long todoItemId) =>
            Notification.CreateError(NotSameIdNotificationId, $"TodoItem {requestId} does not match {todoItemId}.");
    }
}
