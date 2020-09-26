using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tips.ApiMessage.TodoItems.Rules.Engine
{
    internal interface IRulesEngine
    {
        Task Process<TRequest, TResponse>(TRequest request, TResponse response, IEnumerable<BaseRule<TRequest, TResponse>> rules);
    }
}
