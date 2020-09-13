using System.Collections.Generic;

namespace Tips.ApiMessage.TodoItems.Rules
{
    internal interface IRulesFactory
    {
        IEnumerable<BaseRule> Create();
    }
}