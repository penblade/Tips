using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

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
        //     For each modification add info/warning/error messages per rules.
        // The following is very basic.  I may create another article later for complex ETL.
        public List<Notification> ProcessRules(UpdateTodoItemRequest request, TodoItemEntity todoItemEntity)
        {
            var notifications = new List<Notification>();
            if (string.IsNullOrEmpty(request.TodoItem.Name)) notifications.Add(TodoItemNameWasNotProvidedNotification());
            if (string.IsNullOrEmpty(request.TodoItem.Description)) notifications.Add(TodoItemDescriptionWasNotProvidedNotification());
            if (request.TodoItem.Priority < 1 || request.TodoItem.Priority > 3) notifications.Add(TodoItemPriorityIsNotInRangeNotification());

            todoItemEntity.Name = request.TodoItem.Name;
            todoItemEntity.Description = request.TodoItem.Description;
            todoItemEntity.Priority = request.TodoItem.Priority;
            todoItemEntity.IsComplete = request.TodoItem.IsComplete;

            return notifications;
        }
        private static Notification TodoItemNameWasNotProvidedNotification() =>
            new NotificationBuilder()
                .Id("148877DF-F147-413F-97AA-F306A36BCBE1")
                .Severity(Severity.Error)
                .Detail("TodoItem Name was not provided.")
                .Build();

        private static Notification TodoItemDescriptionWasNotProvidedNotification() =>
            new NotificationBuilder()
                .Id("54BD317D-60CD-4BDE-B52D-CF7D0A1D9D38")
                .Severity(Severity.Error)
                .Detail("TodoItem Description was not provided.")
                .Build();

        private static Notification TodoItemPriorityIsNotInRangeNotification() =>
            new NotificationBuilder()
                .Id("54BD317D-60CD-4BDE-B52D-CF7D0A1D9D38")
                .Severity(Severity.Error)
                .Detail("TodoItem Priority must be between 1 - 3.")
                .Build();
    }
}
