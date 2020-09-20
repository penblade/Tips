using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class SaveRulesFactory : IRulesFactory<Request<TodoItem>, Response<TodoItem>>
    {
        public IEnumerable<BaseRule<Request<TodoItem>, Response<TodoItem>>> Create()
        {
            yield return new RequestRule();
            yield return new ResponseRule();
            yield return new TodoItemNameRule();
            yield return new TodoItemDescriptionRule();
            yield return new TodoItemPriorityRule();
        }
    }
}
