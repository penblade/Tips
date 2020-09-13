using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules
{
    internal interface ITodoItemRulesEngine
    {
        void ProcessRules(SaveTodoItemRequest request, Response<TodoItem> response, IEnumerable<BaseRule> rules);
    }
}
