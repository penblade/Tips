using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules
{
    public interface ITodoItemRulesEngine
    {
        void ProcessRules(SaveTodoItemRequest request, Response<TodoItem> response);
    }
}
