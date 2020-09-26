using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tips.ApiMessage.Infrastructure;

namespace Tips.ApiMessage.TodoItems.Rules.Engine
{
    internal abstract class BaseRule<TRequest, TResponse>
    {
        protected List<Type> RequiredRules { get; } = new List<Type>();

        public bool ContinueProcessing { get; protected set; } = true;

        // Template method pattern
        // https://en.wikipedia.org/wiki/Template_method_pattern
        public async Task Process(TRequest request, TResponse response, IEnumerable<BaseRule<TRequest, TResponse>> processedRules)
        {
            Guard.AgainstNull(request, nameof(request));
            Guard.AgainstNull(response, nameof(response));
            Guard.AgainstNull(processedRules, nameof(processedRules));

            if (AllRequiredRulesHavePassed(processedRules))
                await ProcessRule(request, response);
            else
                RuleSkipped();
        }

        private bool AllRequiredRulesHavePassed(IEnumerable<BaseRule<TRequest, TResponse>> processedRules) =>
            RequiredRules.All(requiredRule => processedRules.Any(processedRule => processedRule.GetType() == requiredRule && processedRule.Passed));

        protected abstract Task ProcessRule(TRequest request, TResponse response);

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
