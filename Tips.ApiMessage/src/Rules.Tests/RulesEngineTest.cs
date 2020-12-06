using System.Collections.Generic;
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
        public async Task ProcessAsyncTest()
        {
            var rulesEngine = new RulesEngine();
            var fakeRequest = new FakeRequest();
            var fakeResponse = new FakeResponse();

            var mockRule1 = CreateMockRule(fakeRequest, fakeResponse);
            var mockRule2 = CreateMockRule(fakeRequest, fakeResponse);
            var mockRule3 = CreateMockRule(fakeRequest, fakeResponse);

            var fakeRules = new List<IBaseRule<FakeRequest, FakeResponse>>
            {
                mockRule1.Object,
                mockRule2.Object,
                mockRule3.Object
            };

            await rulesEngine.ProcessAsync(fakeRequest, fakeResponse, fakeRules);

            VerifyRuleCalledOnce(mockRule1, fakeRequest, fakeResponse);
            VerifyRuleCalledOnce(mockRule2, fakeRequest, fakeResponse);
            VerifyRuleCalledOnce(mockRule3, fakeRequest, fakeResponse);
        }

        private static Mock<IBaseRule<FakeRequest, FakeResponse>> CreateMockRule(FakeRequest fakeRequest, FakeResponse fakeResponse)
        {
            var mockRule = new Mock<IBaseRule<FakeRequest, FakeResponse>>();
            mockRule.Setup(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules())).Returns(Task.CompletedTask);
            mockRule.Setup(x => x.Status).Returns(RuleStatusType.Passed);
            mockRule.Setup(x => x.ContinueProcessing).Returns(true);
            return mockRule;
        }

        private static void VerifyRuleCalledOnce(Mock<IBaseRule<FakeRequest, FakeResponse>> mockRule, FakeRequest fakeRequest, FakeResponse fakeResponse) =>
            mockRule.Verify(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules()), Times.Once);

        private static IEnumerable<IBaseRule<FakeRequest, FakeResponse>> ItIsAnyRules() => It.IsAny<IEnumerable<IBaseRule<FakeRequest, FakeResponse>>>();

        public class FakeRequest {}

        public class FakeResponse {}
    }
}
