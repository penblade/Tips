﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Rules.UpdateRules;
using Tips.TodoItems.Tests.Context;
using Tips.TodoItems.Tests.Support;

namespace Tips.TodoItems.Tests.Rules.UpdateRules
{
    [TestClass]
    public class TodoItemNotFoundRuleTest : WithContext
    {
        // Request/Response/RequiredRules null guards are done in the BaseRule.
        // The BaseRule framework has unit tests validating the guards, so no need to do it here again.

        private const int ItemId = 1;
        private const int TotalItems = 1;

        [TestMethod]
        public void IsBaseRule() => VerifyRule.VerifyIsAssignableFrom<BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>, TodoItemNotFoundRule>();

        [TestMethod]
        public async Task ProcessRuleAsyncPass()
        {
            await PopulateTodoItems(TotalItems);
            var request = CreateRequest();
            var response = CreateResponse();

            var rule = new TodoItemNotFoundRule(Context);
            await rule.ProcessAsync(request, response, RuleFactory.CreateEmptyListOfUpdateRules());

            Assert.AreEqual(RuleStatusType.Passed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        [TestMethod]
        public async Task ProcessRuleAsyncTodoItemNotFound()
        {
            var request = CreateRequest();
            var response = CreateResponse();

            var rule = new TodoItemNotFoundRule(Context);
            await rule.ProcessAsync(request, response, RuleFactory.CreateEmptyListOfUpdateRules());

            Assert.IsInstanceOfType(rule, typeof(BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>));
            Assert.AreEqual(RuleStatusType.Failed, rule.Status);
            Assert.IsFalse(rule.ContinueProcessing);

            VerifyNotification.AssertResponseNotifications(CreateExpectedResponse(), response);
        }

        private static UpdateTodoItemRequest CreateRequest() => new() { Id = ItemId, Item = TodoItemFactory.CreateTodoItem(ItemId) };
        private static Response<TodoItemEntity> CreateResponse() => new();

        private static Response CreateExpectedResponse() =>
            new(NotFoundNotification.Create(TodoItemNotFoundRule.TodoItemNotFoundNotificationId, $"TodoItem {ItemId} was not found."));
    }
}
