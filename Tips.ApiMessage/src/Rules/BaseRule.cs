using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tips.Rules
{
    public abstract class BaseRule<TRequest, TResponse> : IBaseRule<TRequest, TResponse>
    {
        protected List<Type> RequiredRules { get; } = new List<Type>();

        public bool ContinueProcessing { get; protected set; } = true;

        // Template method pattern
        // https://en.wikipedia.org/wiki/Template_method_pattern
        public async Task ProcessAsync(TRequest request, TResponse response, IEnumerable<IBaseRule<TRequest, TResponse>> rules)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (rules == null) throw new ArgumentNullException(nameof(rules));

            if (AllRequiredRulesHavePassed(rules))
                await ProcessRuleAsync(request, response);
            else
                Skip();
        }

        private bool AllRequiredRulesHavePassed(IEnumerable<IBaseRule<TRequest, TResponse>> rules) =>
            RequiredRules.All(requiredRule => rules.Any(rule => rule.GetType() == requiredRule && rule.IsPassed()));

        protected abstract Task ProcessRuleAsync(TRequest request, TResponse response);

        public RuleStatusType Status { get; private set; } = RuleStatusType.NotProcessed;

        protected void Skip() => Status = RuleStatusType.Skipped;
        protected void Fail() => Status = RuleStatusType.Failed;
        protected void Pass() => Status = RuleStatusType.Passed;
    }
}
