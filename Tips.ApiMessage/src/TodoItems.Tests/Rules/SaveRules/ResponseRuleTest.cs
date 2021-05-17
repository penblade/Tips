using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;
using Tips.TodoItems.Rules.SaveRules;
using TodoItems.Tests.Support;

namespace TodoItems.Tests.Rules.SaveRules
{
    [TestClass]
    public class ResponseRuleTest
    {
        // Request/Response/RequiredRules null guards are done in the BaseRule.
        // The BaseRule framework has unit tests validating the guards, so no need to do it here again.

        [TestMethod]
        public void IsBaseRule() => VerifyRule.VerifyIsAssignableFrom<BaseRule<Request<TodoItem>, Response<TodoItemEntity>>, ResponseRule>();

        [TestMethod]
        [DynamicData(nameof(SetupProcessRuleAsyncPass), DynamicDataSourceType.Method)]
        public async Task ProcessRuleAsyncPass(string scenario, object expectedItem)
        {
            var expectedTodoItemEntity = (TodoItemEntity) expectedItem;
            var request = CreateRequest((int)expectedTodoItemEntity.Id);
            var response = CreateResponse();

            var rule = new ResponseRule();
            await rule.ProcessAsync(request, response, RuleFactory.CreateEmptyListOfSaveRules());

            VerifyTodoItem.AssertTodoItem(expectedTodoItemEntity, response.Item);

            Assert.AreEqual(RuleStatusType.Passed, rule.Status);
            Assert.IsTrue(rule.ContinueProcessing);

            Assert.AreEqual(0, response.Notifications.Count);
        }

        private static IEnumerable<object[]> SetupProcessRuleAsyncPass()
        {
            yield return new object[] { "Id 1", new TodoItemEntity { Id = 1, IsComplete = true } };
            yield return new object[] { "Id 2", new TodoItemEntity { Id = 2, IsComplete = false } };
            yield return new object[] { "Id 3", new TodoItemEntity { Id = 3, IsComplete = true } };
            yield return new object[] { "Id 4", new TodoItemEntity { Id = 4, IsComplete = false } };
        }

        private static Request<TodoItem> CreateRequest(int id) => new() { Item = TodoItemFactory.CreateTodoItem(id) };
        private static Response<TodoItemEntity> CreateResponse() => new();
    }
}
