using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Rules.UpdateRules;
using TodoItems.Tests.Support;

namespace TodoItems.Tests.Rules.UpdateRules
{
    [TestClass]
    public class TodoItemNotSameIdRuleTest
    {
        private const int ItemId = 1;
        private const int NotSameId = 2;

        [TestMethod]
        public void IsBaseRule() => VerifyRule.VerifyIsAssignableFrom<BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>, TodoItemNotSameIdRule>();

        [TestMethod]
        public async Task ProcessRuleAsyncPass()
        {
            var request = CreateRequest();
            var response = CreateResponse();

            var rule = new TodoItemNotSameIdRule();
            await rule.ProcessAsync(request, response, CreateBaseRules);

            Assert.AreEqual(RuleStatusType.Passed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        [TestMethod]
        public async Task ProcessRuleAsyncNotSameId()
        {
            var request = CreateRequest();
            request.Id = NotSameId;

            var response = CreateResponse();

            var rule = new TodoItemNotSameIdRule();
            await rule.ProcessAsync(request, response, CreateBaseRules);

            Assert.IsInstanceOfType(rule, typeof(BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>));
            Assert.AreEqual(RuleStatusType.Failed, rule.Status);
            Assert.IsFalse(rule.ContinueProcessing);

            AssertNotFoundNotification(response);
        }

        private static void AssertNotFoundNotification(Response<TodoItemEntity> response)
        {
            Assert.AreEqual(1, response.Notifications.Count);
            var notification = response.Notifications.Single();
            Assert.IsInstanceOfType(notification, typeof(Notification));
            Assert.AreEqual(Notification.SeverityType.Error, notification.Severity);
            Assert.AreEqual(TodoItemNotSameIdRule.NotSameIdNotificationId, notification.Id);
            Assert.AreEqual($"TodoItem {NotSameId} does not match {ItemId}.", notification.Detail);
            Assert.AreEqual(0, notification.Notifications.Count);
        }

        private static UpdateTodoItemRequest CreateRequest() => new() { Id = ItemId, Item = TodoItemFactory.CreateTodoItem(ItemId) };
        private static Response<TodoItemEntity> CreateResponse() => new();

        private static IEnumerable<IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>> CreateBaseRules =>
            new List<IBaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>>();
    }
}
