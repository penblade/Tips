using System;
using System.Collections.Generic;

namespace Tips.ApiMessage.TodoItems.Rules.Engine
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
        //     implements a standard Process method defined in an interface
        //     following the strategy pattern.
        //     public void Process(Request request, Response response) { ... }
        //     You want both the request and response passed in 

        // The rules engine then simplifies to accept a rules factory via
        //     constructor injection, loops through each rule calling the
        //     Process method, and then returns the final response.
        public void Process<TRequest, TResponse>(TRequest request, TResponse response, IEnumerable<BaseRule<TRequest, TResponse>> rules)
        {
            var processedRules = new List<Type>();
            foreach (var rule in rules)
            {
                rule.Process(request, response, processedRules);
                processedRules.Add(rule.GetType());
                if (!rule.ContinueProcessing) return;
            }
        }
    }
}
