using System.Collections.Generic;
using Tips.ApiMessage.TodoItems.Rules.SaveRules;

namespace Tips.ApiMessage.TodoItems.Rules
{
    internal class SaveRulesFactory : IRulesFactory
    {
        public IEnumerable<BaseRule> Create()
        {
            yield return new TodoItemDefaultsRule();
            yield return new TodoItemNameRule();
            yield return new TodoItemDescriptionRule();
            yield return new TodoItemPriorityRule();
        }
    }
}
