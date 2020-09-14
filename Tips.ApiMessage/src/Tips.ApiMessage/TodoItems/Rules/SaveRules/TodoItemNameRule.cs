﻿using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemNameRule : BaseRule<SaveTodoItemRequest, Response<TodoItem>>
    {
        public TodoItemNameRule() => RequiredRules.AddRange(new []{typeof(RequestRule), typeof(ResponseRule)});

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
