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
        public void ProcessAsyncTest()
        {
            var rulesEngine = new RulesEngine();
            var fakeRequest = new FakeRequest();
            var fakeResponse = new FakeResponse();

            var mockRule1 = CreateMockRule(fakeRequest, fakeResponse);
            var mockRule2 = CreateMockRule(fakeRequest, fakeResponse);
            var mockRule3 = CreateMockRule(fakeRequest, fakeResponse);

            var fakeRules = new List<BaseRule<FakeRequest, FakeResponse>>
            {
                mockRule1.Object,
                mockRule2.Object,
                mockRule3.Object
            };

            var actualRules = rulesEngine.ProcessAsync(fakeRequest, fakeResponse, fakeRules).Result.ToList();

            VerifyRuleCalledOnce(mockRule1, fakeRequest, fakeResponse);
            VerifyRuleCalledOnce(mockRule2, fakeRequest, fakeResponse);
            VerifyRuleCalledOnce(mockRule3, fakeRequest, fakeResponse);

            var count = fakeRules.Count;
            for (var i = 0; i < count; i++)
            {
                Assert.AreSame(fakeRules[i], actualRules[i]);
            }
        }

        private static Mock<BaseRule<FakeRequest, FakeResponse>> CreateMockRule(FakeRequest fakeRequest, FakeResponse fakeResponse)
        {
            var mockRule = new Mock<BaseRule<FakeRequest, FakeResponse>>();
            mockRule.Setup(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules())).Returns(Task.CompletedTask);
            return mockRule;
        }

        private static void VerifyRuleCalledOnce(Mock<BaseRule<FakeRequest, FakeResponse>> mockRule, FakeRequest fakeRequest, FakeResponse fakeResponse) =>
            mockRule.Verify(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules()), Times.Once);

        private static IEnumerable<BaseRule<FakeRequest, FakeResponse>> ItIsAnyRules() => It.IsAny<IEnumerable<BaseRule<FakeRequest, FakeResponse>>>();

        public class FakeRequest {}

        public class FakeResponse {}
    }
}
