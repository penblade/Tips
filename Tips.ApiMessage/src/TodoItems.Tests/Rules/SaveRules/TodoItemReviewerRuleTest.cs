using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class TodoItemReviewerRuleTest
    {
        // Request/Response/RequiredRules null guards are done in the BaseRule.
        // The BaseRule framework has unit tests validating the guards, so no need to do it here again.

        private const int ItemId = 1;

        [TestMethod]
        public void IsBaseRule() => VerifyRule.VerifyIsAssignableFrom<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>, TodoItemReviewerRule>();

        [TestMethod]
        [DynamicData(nameof(SetupProcessRuleAsyncSkipped), DynamicDataSourceType.Method)]
        public async Task ProcessRuleAsyncSkipped(string scenario, object expectedRules)
        {
            var rules = (List<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>)expectedRules;
            var request = CreateRequest();
            var response = CreateResponse();

            var rule = new TodoItemReviewerRule();
            await rule.ProcessAsync(request, response, rules);

            Assert.AreEqual(RuleStatusType.Skipped, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        private static IEnumerable<object[]> SetupProcessRuleAsyncSkipped()
        {
            var rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            yield return new object[] { "Rules: 0 of 3 required rules, 0 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            yield return new object[] { "Rules: 0 of 3 required rules, 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<RequestRule>());
            yield return new object[] { "Rules: 1 of 3 required rules (RequestRule), 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<ResponseRule>());
            yield return new object[] { "Rules: 1 of 3 required rules (ResponseRule), 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<TodoItemPriorityRule>());
            yield return new object[] { "Rules: 1 of 3 required rules (TodoItemPriorityRule), 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<RequestRule>());
            rules.Add(RuleFactory.CreatePassedRule<ResponseRule>());
            yield return new object[] { "Rules: 2 of 3 required rules (RequestRule, ResponseRule), 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<RequestRule>());
            rules.Add(RuleFactory.CreatePassedRule<TodoItemPriorityRule>());
            yield return new object[] { "Rules: 2 of 3 required rules (RequestRule, TodoItemPriorityRule), 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<ResponseRule>());
            rules.Add(RuleFactory.CreatePassedRule<TodoItemPriorityRule>());
            yield return new object[] { "Rules: 2 of 3 required rules (ResponseRule, TodoItemPriorityRule), 1 MockRule", rules };
        }

        [TestMethod]
        [DataRow(1, "Peter")]
        [DataRow(2, "Lois")]
        [DataRow(3, "Brian")]
        public async Task ProcessRuleAsyncPass(int priority, string expectedReviewer)
        {
            var request = CreateRequest();
            request.Item.Priority = priority;
            var response = CreateResponse();

            var rule = new TodoItemReviewerRule();
            await rule.ProcessAsync(request, response, CreateBaseRulesWithRequiredRules());

            Assert.AreEqual(expectedReviewer, response.Item.Reviewer);

            Assert.AreEqual(RuleStatusType.Passed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public async Task ProcessRuleAsyncPriorityIsNotInRange(int priority)
        {
            var request = CreateRequest();
            request.Item.Priority = priority;
            var response = CreateResponse();

            var rule = new TodoItemReviewerRule();
            await rule.ProcessAsync(request, response, CreateBaseRulesWithRequiredRules());

            Assert.AreEqual(null, response.Item.Reviewer);

            Assert.IsInstanceOfType(rule, typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>));
            Assert.AreEqual(RuleStatusType.Failed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            VerifyNotification.AssertResponseNotifications(CreateExpectedResponse(priority), response);
        }

        private static Request<TodoItem> CreateRequest() => new() { Item = TodoItemFactory.CreateTodoItem(ItemId) };
        private static Response<TodoItemEntity> CreateResponse() => new();

        private static Response CreateExpectedResponse(int priority)
        {
            var notification = Notification.CreateError(TodoItemReviewerRule.TodoItemReviewerIsNullNotificationId, "TodoItem Reviewer could not be determined.");
            notification.Notifications.Add(Notification.CreateInfo(TodoItemReviewerRule.TodoItemReviewerIsNullReason1NotificationId, "Reviewer is based on priority."));
            notification.Notifications.Add(Notification.CreateInfo(TodoItemReviewerRule.TodoItemReviewerIsNullReason2NotificationId, $"An administrator needs to assign a reviewer to priority {priority}."));
            notification.Notifications.Add(Notification.CreateInfo(TodoItemReviewerRule.TodoItemReviewerIsNullReason3NotificationId, "Please contact your administrator with this error."));
            notification.Notifications.Add(Notification.CreateInfo(TodoItemReviewerRule.TodoItemReviewerIsNullReason4NotificationId, "This is an example of nested notifications."));

            return new Response(notification);
        }

        private static IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>> CreateBaseRulesWithRequiredRules()
        {
            var requiredRules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            requiredRules.Add(RuleFactory.CreatePassedRule<RequestRule>());
            requiredRules.Add(RuleFactory.CreatePassedRule<ResponseRule>());
            requiredRules.Add(RuleFactory.CreatePassedRule<TodoItemPriorityRule>());
            return requiredRules;
        }
    }
}
