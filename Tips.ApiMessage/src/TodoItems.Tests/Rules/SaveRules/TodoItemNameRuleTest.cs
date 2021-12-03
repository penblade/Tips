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
    public class TodoItemNameRuleTest
    {
        // Request/Response/RequiredRules null guards are done in the BaseRule.
        // The BaseRule framework has unit tests validating the guards, so no need to do it here again.

        private const int ItemId = 1;

        [TestMethod]
        public void IsBaseRule() => VerifyRule.VerifyIsAssignableFrom<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>, TodoItemNameRule>();

        [TestMethod]
        [DynamicData(nameof(SetupProcessRuleAsyncSkipped), DynamicDataSourceType.Method)]
        public async Task ProcessRuleAsyncSkipped(string scenario, object expectedRules)
        {
            var rules = (List<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>>)expectedRules;
            var request = CreateRequest();
            var response = CreateResponse();

            var rule = new TodoItemNameRule();
            await rule.ProcessAsync(request, response, rules);

            Assert.AreEqual(RuleStatusType.Skipped, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        private static IEnumerable<object[]> SetupProcessRuleAsyncSkipped()
        {
            var rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            yield return new object[] { "Rules: 0 of 2 required rules, 0 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            yield return new object[] { "Rules: 0 of 2 required rules, 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<RequestRule>());
            yield return new object[] { "Rules: 1 of 2 required rules (RequestRule), 1 MockRule", rules };

            rules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            rules.Add(RuleFactory.CreateMockSaveRule());
            rules.Add(RuleFactory.CreatePassedRule<ResponseRule>());
            yield return new object[] { "Rules: 1 of 2 required rules (ResponseRule), 1 MockRule", rules };
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        public async Task ProcessRuleAsyncPass(int id)
        {
            var request = CreateRequest(id);
            var response = CreateResponse();

            var rule = new TodoItemNameRule();
            await rule.ProcessAsync(request, response, CreateBaseRulesWithRequiredRules());

            Assert.AreEqual($"TodoItem - Name - {id}", response.Item.Name);

            Assert.AreEqual(RuleStatusType.Passed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task ProcessRuleAsyncNameNotProvided(string name)
        {
            var request = CreateRequest();
            request.Item.Name = name;
            var response = CreateResponse();

            var rule = new TodoItemNameRule();
            await rule.ProcessAsync(request, response, CreateBaseRulesWithRequiredRules());

            Assert.IsInstanceOfType(rule, typeof(BaseRule<Request<TodoItem>, Response<TodoItemEntity>>));
            Assert.AreEqual(RuleStatusType.Failed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            VerifyNotification.AssertResponseNotifications(CreateExpectedResponse(), response);
        }

        private static Request<TodoItem> CreateRequest(int id) => new() { Item = TodoItemFactory.CreateTodoItem(id) };
        private static Request<TodoItem> CreateRequest() => new() { Item = TodoItemFactory.CreateTodoItem(ItemId) };
        private static Response<TodoItemEntity> CreateResponse() => new();

        private static Response CreateExpectedResponse() =>
            new(Notification.CreateError(TodoItemNameRule.TodoItemNameWasNotProvidedNotificationId, "TodoItem Name was not provided."));

        private static IEnumerable<IBaseRule<Request<TodoItem>, Response<TodoItemEntity>>> CreateBaseRulesWithRequiredRules()
        {
            var requiredRules = RuleFactory.CreateEmptyListOfSaveRules().ToList();
            requiredRules.Add(RuleFactory.CreatePassedRule<RequestRule>());
            requiredRules.Add(RuleFactory.CreatePassedRule<ResponseRule>());
            return requiredRules;
        }
    }
}
