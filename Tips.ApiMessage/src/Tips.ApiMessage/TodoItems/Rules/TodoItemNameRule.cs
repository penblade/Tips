﻿using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules
{
    internal class TodoItemNameRule : BaseRule
    {
        protected override void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            if (string.IsNullOrEmpty(request.TodoItem.Name))
            {
                response.Add(TodoItemNameWasNotProvidedNotification());
                return;
            }

            response.Result.Name = request.TodoItem.Name;
        }

        internal const string TodoItemNameWasNotProvidedNotificationId = "148877DF-F147-413F-97AA-F306A36BCBE1";
        private static Notification TodoItemNameWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemNameWasNotProvidedNotificationId, "TodoItem Name was not provided.");
    }
}
