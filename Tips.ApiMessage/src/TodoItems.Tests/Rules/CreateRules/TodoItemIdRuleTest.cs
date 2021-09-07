using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.CreateTodoItem;
using Tips.TodoItems.Rules.CreateRules;
using TodoItems.Tests.Support;

namespace TodoItems.Tests.Rules.CreateRules
{
    [TestClass]
    public class TodoItemIdRuleTest
    {
        // Request/Response/RequiredRules null guards are done in the BaseRule.
        // The BaseRule framework has unit tests validating the guards, so no need to do it here again.

        private const int NewItemId = 0;
        private const int ItemId = 1;

        [TestMethod]
        public void IsBaseRule() => VerifyRule.VerifyIsAssignableFrom<BaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>, TodoItemIdRule>();

        [TestMethod]
        public async Task ProcessRuleAsyncPass()
        {
            var request = CreateRequest(NewItemId);
            var response = CreateResponse();

            var rule = new TodoItemIdRule();
            await rule.ProcessAsync(request, response, RuleFactory.CreateEmptyListOfCreateRules());

            Assert.AreEqual(RuleStatusType.Passed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        [TestMethod]
        public async Task ProcessRuleAsyncNotProvided()
        {
            var request = CreateRequest(ItemId);
            var response = CreateResponse();

            var rule = new TodoItemIdRule();
            await rule.ProcessAsync(request, response, RuleFactory.CreateEmptyListOfCreateRules());

            Assert.IsInstanceOfType(rule, typeof(BaseRule<CreateTodoItemRequest, Response<TodoItemEntity>>));
            Assert.AreEqual(RuleStatusType.Failed, rule.Status);
            Assert.IsFalse(rule.ContinueProcessing);

            VerifyNotification.AssertResponseNotifications(CreateExpectedResponse(), response);
        }

        private static CreateTodoItemRequest CreateRequest(int id) => new() { Item = TodoItemFactory.CreateTodoItem(id) };
        private static Response<TodoItemEntity> CreateResponse() => new();

        private static Response CreateExpectedResponse() =>
            new(Notification.CreateError(TodoItemIdRule.TodoItemIdIsNotEmptyRuleNotificationId, "Do not send the TodoItem.ID when creating a new TodoItem."));
    }
}
