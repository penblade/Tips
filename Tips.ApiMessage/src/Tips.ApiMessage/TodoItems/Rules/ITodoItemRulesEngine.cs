using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules
{
    public interface ITodoItemRulesEngine
    {
        List<Notification> ProcessRules(SaveTodoItemRequest request, TodoItemEntity todoItemEntity);
    }
}
