using System.Collections.Generic;

namespace Tips.ApiMessage.TodoItems.Rules.Engine
{
    internal interface IRulesEngine
    {
        void Process<TRequest, TResponse>(TRequest request, TResponse response, IEnumerable<BaseRule<TRequest, TResponse>> rules);
    }
}
