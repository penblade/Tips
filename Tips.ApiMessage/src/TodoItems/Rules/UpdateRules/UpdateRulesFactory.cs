using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;

namespace Tips.TodoItems.Rules.UpdateRules
{
    public class UpdateRulesFactory : IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateRulesFactory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public IEnumerable<BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>> Create() =>
            _serviceProvider.GetServices<BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>>();
    }
}
