using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tips.Rules
{
    public interface IRulesEngine
    {
        /// <summary>
        /// Processes the rules.  A rule may decide that the engine should stop processing rules.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="rules"></param>
        /// <returns>List of the rules that were processed.</returns>
        Task ProcessAsync<TRequest, TResponse>(TRequest request, TResponse response, IEnumerable<IBaseRule<TRequest, TResponse>> rules);
    }
}
