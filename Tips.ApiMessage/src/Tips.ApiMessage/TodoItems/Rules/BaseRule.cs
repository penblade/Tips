using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Infrastructure;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules
{
    internal abstract class BaseRule
    {
        protected List<Type> RequiredRules { get; } = new List<Type>();

        // Template method pattern
        // https://en.wikipedia.org/wiki/Template_method_pattern
        public void Process(SaveTodoItemRequest request, Response<TodoItem> response, IEnumerable<Type> processedRules)
        {
            if (!AllRequiredRulesHaveBeenProcessed(processedRules)) ThrowMissingRulesException();

            ProcessRule(request, response);
        }

        private void ThrowMissingRulesException()
        {
            var message = string.Join(", ", RequiredRules.Select(rule => rule.FullName));
            throw new Exception($"Missing required rules: {message}");
        }

        private bool AllRequiredRulesHaveBeenProcessed(IEnumerable<Type> processedRules)
        {
            Guard.AgainstNull(processedRules, nameof(processedRules));
            return RequiredRules.All(requiredRule => processedRules.Any(processedRule => processedRule == requiredRule));
        }

        protected abstract void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response);
    }
}
