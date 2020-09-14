using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class SaveRulesFactory : IRulesFactory<SaveTodoItemRequest, Response<TodoItem>>
    {
        public IEnumerable<BaseRule<SaveTodoItemRequest, Response<TodoItem>>> Create()
        {
            yield return new RequestRule();
            yield return new ResponseRule();
            yield return new TodoItemNameRule();
            yield return new TodoItemDescriptionRule();
            yield return new TodoItemPriorityRule();
        }
    }
}
