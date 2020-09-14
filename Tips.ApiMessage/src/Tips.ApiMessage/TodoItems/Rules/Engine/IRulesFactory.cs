using System.Collections.Generic;

namespace Tips.ApiMessage.TodoItems.Rules.Engine
{
    internal interface IRulesFactory<TRequest, TResponse>
    {
        IEnumerable<BaseRule<TRequest, TResponse>> Create();
    }
}