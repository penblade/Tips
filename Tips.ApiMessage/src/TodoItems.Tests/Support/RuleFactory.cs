using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.CreateTodoItem;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Tests.Support
{
    internal static class RuleFactory
    {
        public static IEnumerable<IBaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>> CreateEmptyListOfCreateRules() =>
            new List<IBaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>>();

        public static IEnumerable<IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>> CreateEmptyListOfUpdateRules() =>
            new List<IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>>();

        public static IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>> CreateEmptyListOfSaveRules() =>
            new List<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>();

        public static IBaseRule<Request<TodoItem>, Response<TodoItemEntity>> CreateMockSaveRule() =>
            new Mock<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>().Object;

        public static IBaseRule<CreateTodoItemRequest, Response<TodoItemEntity>> CreateMockCreateRule() =>
            new Mock<IBaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>>().Object;

        public static IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>> CreateMockUpdateRule() =>
            new Mock<IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>>().Object;

        private static Type BaseType => typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>);
        public static TRule CreatePassedRule<TRule>()
        {
            var rule = (TRule)Activator.CreateInstance(typeof(TRule));
            var property1 = BaseType?.GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
            property1?.SetValue(rule, RuleStatusType.Passed);
            return rule;
        }
    }
}
