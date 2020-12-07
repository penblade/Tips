using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Rules;

namespace Rules.Tests
{
    [TestClass]
    public class RulesEngineTest
    {
        [TestMethod]
        [DynamicData(nameof(SetupProcessAsyncTest), DynamicDataSourceType.Method)]
        public async Task ProcessAsyncTest(FakeRequest fakeRequest, FakeResponse fakeResponse, IList<Mock<IBaseRule<FakeRequest, FakeResponse>>> mockRules)
        {
            var rulesEngine = new RulesEngine();
            await rulesEngine.ProcessAsync(fakeRequest, fakeResponse, mockRules.Select(mockRule => mockRule.Object));

            foreach (var mockRule in mockRules)
            {
                VerifyRuleCalledOnce(mockRule, fakeRequest, fakeResponse);
            }
        }

        private static IEnumerable<object[]> SetupProcessAsyncTest()
        {
            var fakeRequest = new FakeRequest();
            var fakeResponse = new FakeResponse();

            yield return new object[]
            {
                fakeRequest,
                fakeResponse,
                new List<Mock<IBaseRule<FakeRequest, FakeResponse>>>
                {
                    CreateMockRule(fakeRequest, fakeResponse, RuleStatusType.Passed, true),
                    CreateMockRule(fakeRequest, fakeResponse, RuleStatusType.Passed, true),
                    CreateMockRule(fakeRequest, fakeResponse, RuleStatusType.Passed, true)
                }
            };
        }

        private static Mock<IBaseRule<FakeRequest, FakeResponse>>
            CreateMockRule(FakeRequest fakeRequest, FakeResponse fakeResponse, RuleStatusType statusType, bool continueProcessing)
        {
            var mockRule = new Mock<IBaseRule<FakeRequest, FakeResponse>>();
            mockRule.Setup(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules())).Returns(Task.CompletedTask);
            mockRule.Setup(x => x.Status).Returns(statusType);
            mockRule.Setup(x => x.ContinueProcessing).Returns(continueProcessing);
            return mockRule;
        }

        private static void VerifyRuleCalledOnce(Mock<IBaseRule<FakeRequest, FakeResponse>> mockRule, FakeRequest fakeRequest, FakeResponse fakeResponse) =>
            mockRule.Verify(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules()), Times.Once);

        private static IEnumerable<IBaseRule<FakeRequest, FakeResponse>> ItIsAnyRules() => It.IsAny<IEnumerable<IBaseRule<FakeRequest, FakeResponse>>>();

        public class FakeRequest {}

        public class FakeResponse {}
    }
}
