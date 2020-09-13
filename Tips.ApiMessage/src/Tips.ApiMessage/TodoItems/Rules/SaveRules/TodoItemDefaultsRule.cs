using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class TodoItemDefaultsRule : BaseRule
    {
        protected override void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            response.Result.Id = request.TodoItem.Id;
            response.Result.IsComplete = request.TodoItem.IsComplete;
        }
    }
}
