using System.Collections.Generic;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Rules.SaveRules
{
    public class SaveRulesFactory : IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>
    {
        public IEnumerable<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>> Create()
        {
            yield return new RequestRule();
            yield return new ResponseRule();
            yield return new TodoItemNameRule();
            yield return new TodoItemDescriptionRule();
            yield return new TodoItemPriorityRule();
            yield return new TodoItemReviewerRule();
        }
    }
}
