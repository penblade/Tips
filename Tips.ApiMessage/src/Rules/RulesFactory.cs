using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Tips.Rules
{
    internal class RulesFactory<TRequest, TResponse> : IRulesFactory<TRequest, TResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        public RulesFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IEnumerable<BaseRule<TRequest, TResponse>> Create() => _serviceProvider.GetServices<BaseRule<TRequest, TResponse>>();
    }
}
