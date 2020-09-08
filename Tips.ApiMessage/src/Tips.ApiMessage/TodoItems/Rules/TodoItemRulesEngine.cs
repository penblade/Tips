﻿using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Infrastructure;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules
{
    internal class TodoItemRulesEngine : ITodoItemRulesEngine
    {
        // With complex Extract/Transform/Load (ETL) operations, mapping isn't necessarily 1-1 and adjustments must be done.
        // A rules engine can separate concerns for ETL from the domain object.
        // Validate against the source.
        //     Add info/warning/error messages per rules.
        // Map the source to the target and modify data in the target as necessary.
        //     Do not modify the source.  Add helper/relational/reference objects as necessary.
        //     Depend on the source data, not the current state of the response
        //     to avoid side effects as you move rules around.
        //     For each modification add info/warning/error messages per rules.
        // The following is very basic.

        // For more complex ETL each rule refactored into it's own class that
        //     implements a standard Process method defined in an interface
        //     following the strategy pattern.
        //     public void Process(Request request, Response response) { ... }
        //     You want both the request and response passed in 

        // The rules engine then simplifies to accept a rules factory via
        //     constructor injection, loops through each rule calling the
        //     Process method, and then returns the final response.
        public void ProcessRules(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            Guard.AgainstNull(request, "request");
            Guard.AgainstNull(request.TodoItem, "request.TodoItem");
            Guard.AgainstNull(response, "response");

            response.Result = new TodoItem
            {
                Id = request.TodoItem.Id,
                IsComplete = request.TodoItem.IsComplete
            };

            TodoItemNameRule(request, response);
            TodoItemDescriptionRule(request, response);
            TodoItemPriorityRule(request, response);
        }

        private static void TodoItemNameRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            Guard.AgainstNull(request, "request");
            Guard.AgainstNull(request.TodoItem, "request.TodoItem");
            Guard.AgainstNull(response, "response");
            Guard.AgainstNull(response.Result, "response.Result");

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

        private static void TodoItemDescriptionRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            Guard.AgainstNull(request, "request");
            Guard.AgainstNull(request.TodoItem, "request.TodoItem");
            Guard.AgainstNull(response, "response");
            Guard.AgainstNull(response.Result, "response.Result");

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

        private static void TodoItemPriorityRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            Guard.AgainstNull(request, "request");
            Guard.AgainstNull(request.TodoItem, "request.TodoItem");
            Guard.AgainstNull(response, "response");
            Guard.AgainstNull(response.Result, "response.Result");

            if (request.TodoItem.Priority < 1 || request.TodoItem.Priority > 3)
            {
                response.Add(TodoItemPriorityIsNotInRangeNotification());
                return;
            }

            response.Result.Priority = request.TodoItem.Priority;
        }

        internal const string TodoItemPriorityIsNotInRangeNotificationId = "C5E1E6F4-D241-4D82-A4C5-832E3C6C1816";
        private static Notification TodoItemPriorityIsNotInRangeNotification() =>
            Notification.CreateError(TodoItemPriorityIsNotInRangeNotificationId, "TodoItem Priority must be between 1 - 3.");
    }
}
