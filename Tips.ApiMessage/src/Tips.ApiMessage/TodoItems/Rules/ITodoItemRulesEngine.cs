using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Rules
{
    public interface ITodoItemRulesEngine
    {
        List<Notification> ProcessRules(UpdateTodoItemRequest request, TodoItemEntity todoItemEntity);
    }
}
