using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemDefaultsRule : BaseRule<SaveTodoItemRequest, Response<TodoItem>>
    {
        protected override void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            response.Result = new TodoItem
            {
                Id = request.TodoItem.Id,
                IsComplete = request.TodoItem.IsComplete
            };
        }
    }
}
