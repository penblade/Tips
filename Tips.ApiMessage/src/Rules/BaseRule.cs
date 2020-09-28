using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tips.Rules
{
    public abstract class BaseRule<TRequest, TResponse>
    {
        protected List<Type> RequiredRules { get; } = new List<Type>();

        public bool ContinueProcessing { get; protected set; } = true;

        // Template method pattern
        // https://en.wikipedia.org/wiki/Template_method_pattern
        public async Task ProcessAsync(TRequest request, TResponse response, IEnumerable<BaseRule<TRequest, TResponse>> processedRules)
        {
            if (request == null) throw new ArgumentException(nameof(request));
            if (response == null) throw new ArgumentException(nameof(response));
            if (processedRules == null) throw new ArgumentException(nameof(processedRules));

            if (AllRequiredRulesHavePassed(processedRules))
                await ProcessRuleAsync(request, response);
            else
                RuleSkipped();
        }

        private bool AllRequiredRulesHavePassed(IEnumerable<BaseRule<TRequest, TResponse>> processedRules) =>
            RequiredRules.All(requiredRule => processedRules.Any(processedRule => processedRule.GetType() == requiredRule && processedRule.Passed));

        protected abstract Task ProcessRuleAsync(TRequest request, TResponse response);

        public bool Skipped => _status == RuleStatusType.Skipped;
        public bool Failed => _status == RuleStatusType.Failed;
        public bool Passed => _status == RuleStatusType.Passed;

        protected void RuleSkipped() => _status = RuleStatusType.Skipped;
        protected void RuleFailed() => _status = RuleStatusType.Failed;
        protected void RulePassed() => _status = RuleStatusType.Passed;

        private RuleStatusType _status = RuleStatusType.NotProcessed;
        private enum RuleStatusType { NotProcessed, Skipped, Failed, Passed }
    }
}
