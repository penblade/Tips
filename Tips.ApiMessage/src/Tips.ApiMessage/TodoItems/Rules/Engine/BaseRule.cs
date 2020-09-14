using System;
using System.Collections.Generic;
using System.Linq;
using Tips.ApiMessage.Infrastructure;

namespace Tips.ApiMessage.TodoItems.Rules.Engine
{
    internal abstract class BaseRule<TRequest, TResponse>
    {
        protected List<Type> RequiredRules { get; } = new List<Type>();
        public bool ContinueProcessing { get; protected set; } = true;

        // Template method pattern
        // https://en.wikipedia.org/wiki/Template_method_pattern
        public void Process(TRequest request, TResponse response, IEnumerable<Type> processedRules)
        {
            Guard.AgainstNull(request, nameof(request));
            Guard.AgainstNull(response, nameof(response));
            Guard.AgainstNull(processedRules, nameof(processedRules));

            if (!AllRequiredRulesHaveBeenProcessed(processedRules)) ThrowMissingRulesException();

            ProcessRule(request, response);
        }

        private void ThrowMissingRulesException()
        {
            var message = string.Join(", ", RequiredRules.Select(rule => rule.FullName));
            throw new Exception($"Missing required rules: {message}");
        }

        private bool AllRequiredRulesHaveBeenProcessed(IEnumerable<Type> processedRules) =>
            RequiredRules.All(requiredRule => processedRules.Any(processedRule => processedRule == requiredRule));

        protected abstract void ProcessRule(TRequest request, TResponse response);
    }
}
