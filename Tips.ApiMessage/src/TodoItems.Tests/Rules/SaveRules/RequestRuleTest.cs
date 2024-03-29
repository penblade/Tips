﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;
using Tips.TodoItems.Rules.SaveRules;
using Tips.TodoItems.Tests.Support;

namespace Tips.TodoItems.Tests.Rules.SaveRules
{
    [TestClass]
    public class RequestRuleTest
    {
        // Request/Response/RequiredRules null guards are done in the BaseRule.
        // The BaseRule framework has unit tests validating the guards, so no need to do it here again.

        private const int ItemId = 1;

        [TestMethod]
        public void IsBaseRule() => VerifyRule.VerifyIsAssignableFrom<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>, RequestRule>();

        [TestMethod]
        public async Task ProcessRuleAsyncPass()
        {
            var request = CreateRequest();
            var response = CreateResponse();

            var rule = new RequestRule();
            await rule.ProcessAsync(request, response, RuleFactory.CreateEmptyListOfSaveRules());

            Assert.AreEqual(RuleStatusType.Passed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        [TestMethod]
        public async Task ProcessRuleAsyncNotProvided()
        {
            var request = CreateRequestWithNoItem();
            var response = CreateResponse();

            var rule = new RequestRule();
            await rule.ProcessAsync(request, response, RuleFactory.CreateEmptyListOfSaveRules());

            Assert.IsInstanceOfType(rule, typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>));
            Assert.AreEqual(RuleStatusType.Failed, rule.Status);
            Assert.IsFalse(rule.ContinueProcessing);

            VerifyNotification.AssertResponseNotifications(CreateExpectedResponse(), response);
        }

        private static Request<TodoItem> CreateRequestWithNoItem() => new() { Item = null };
        private static Request<TodoItem> CreateRequest() => new() { Item = TodoItemFactory.CreateTodoItem(ItemId) };
        private static Response<TodoItemEntity> CreateResponse() => new();

        private static Response CreateExpectedResponse() =>
            new(Notification.CreateError(RequestRule.TodoItemWasNotProvidedNotificationId, "TodoItem was not provided."));
    }
}
