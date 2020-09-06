using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Mappers;
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
            TodoItemNameWasNotProvidedRule(request, response);
            TodoItemDescriptionWasNotProvidedRule(request, response);
            TodoItemPriorityIsNotInRangeRule(request, response);
            MapTodoItemRule(request, response);
        }

        private static void TodoItemNameWasNotProvidedRule(SaveTodoItemRequest request, Response response)
        {
            if (string.IsNullOrEmpty(request.TodoItem.Name))
                response.Add(TodoItemNameWasNotProvidedNotification());
        }

        internal const string TodoItemNameWasNotProvidedNotificationId = "148877DF-F147-413F-97AA-F306A36BCBE1";
        private static Notification TodoItemNameWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemNameWasNotProvidedNotificationId, "TodoItem Name was not provided.");

        private static void TodoItemDescriptionWasNotProvidedRule(SaveTodoItemRequest request, Response response)
        {
            if (string.IsNullOrEmpty(request.TodoItem.Description))
                response.Add(TodoItemDescriptionWasNotProvidedNotification());
        }

        internal const string TodoItemDescriptionWasNotProvidedNotificationId = "54BD317D-60CD-4BDE-B52D-CF7D0A1D9D38";
        private static Notification TodoItemDescriptionWasNotProvidedNotification() =>
            Notification.CreateError(TodoItemDescriptionWasNotProvidedNotificationId, "TodoItem Description was not provided.");

        private static void TodoItemPriorityIsNotInRangeRule(SaveTodoItemRequest request, Response response)
        {
            if (request.TodoItem.Priority < 1 || request.TodoItem.Priority > 3)
                response.Add(TodoItemPriorityIsNotInRangeNotification());
        }

        internal const string TodoItemPriorityIsNotInRangeNotificationId = "C5E1E6F4-D241-4D82-A4C5-832E3C6C1816";
        private static Notification TodoItemPriorityIsNotInRangeNotification() =>
            Notification.CreateError(TodoItemPriorityIsNotInRangeNotificationId, "TodoItem Priority must be between 1 - 3.");

        private static void MapTodoItemRule(SaveTodoItemRequest request, Response<TodoItem> response) =>
            response.Result = GenericMapper.Map<TodoItem, TodoItem>(request.TodoItem);
    }
}
