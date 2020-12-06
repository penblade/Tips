using System.Collections.Generic;

namespace Tips.Rules
{
    public interface IRulesFactory<TRequest, TResponse>
    {
        IEnumerable<IBaseRule<TRequest, TResponse>> Create();
    }
}