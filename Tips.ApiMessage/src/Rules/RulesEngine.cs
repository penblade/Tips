using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tips.Rules
{
    internal class RulesEngine : IRulesEngine
    {
        // With complex Extract/Transform/Load (ETL) operations, mapping isn't necessarily 1-1 and adjustments must be done.
        // A rules engine can separate concerns for ETL from the domain object.
        // Validate against the source.
        //     Add info/warning/error messages per rules.
        // Map the source to the target and modify data in the target as necessary.
        //     Do not modify the source.  Add helper/relational/reference objects as necessary.
        //     Depend on the source data, not the current state of the response
        //     to avoid side effects as you move rules around.
        //     For each modification add info/warning/error messages per rules.
        // The following is very basic.

        // For more complex ETL each rule refactored into it's own class that
        //     implements a standard ProcessAsync method defined in an interface
        //     following the strategy pattern.
        //     public void ProcessAsync(Request request, Response response) { ... }
        //     You want both the request and response passed in 

        // The rules engine then simplifies to accept a rules factory via
        //     constructor injection, loops through each rule calling the
        //     ProcessAsync method, and then returns the final response.
        public async Task ProcessAsync<TRequest, TResponse>(TRequest request, TResponse response, IEnumerable<IBaseRule<TRequest, TResponse>> rules)
        {
            var rulesList = rules.ToList();
            foreach (var rule in rulesList)
            {
                await rule.ProcessAsync(request, response, rulesList);
                if (!rule.ContinueProcessing) return;
            }
        }
    }
}
