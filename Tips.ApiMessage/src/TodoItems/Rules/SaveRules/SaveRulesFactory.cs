using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Rules.SaveRules
{
    internal class SaveRulesFactory : IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>>
    {
        private readonly IServiceProvider _serviceProvider;

        public SaveRulesFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IEnumerable<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>> Create() =>
            _serviceProvider.GetServices<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>>();
    }
}
